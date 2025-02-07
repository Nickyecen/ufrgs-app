using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

// Utilizada para facilitar o manuseio das respostas de queries http
public struct Response {
	public string text {get;}
	public int code {get;}

	public Response(string text, int code) {
		this.text = text;
		this.code = code;
	}

}

// Classe responsável por realizar as conexões ao site da UFRGS
[GlobalClass] public partial class UfrgsConnector : Node {

	[Signal] public delegate void ConnectedEventHandler(); // Emitido em conexão bem sucedida
	[Signal] public delegate void ConnectionErrorEventHandler(); // Emitido em conexão mal sucedida

	const string UFRGS_URL = "https://www1.ufrgs.br";
	const string PORTAL = "/sistemas/portal/";

	// Acesso teste realizado para testar se a autenticação foi bem sucedida
	// Utilizado devido ao portal devolver 200 para ambos os casos
	const string TEST = "/graduacao/xInformacoesAcademicasdoAluno/teste_intranet.php?ret=%2Fintranet%2Fportal%2Fpublic%2Fproxy";

	HttpClient client;

	public override void _Ready() {
		client = HttpClientSingleton.GetInstance().client;
	}

	// Realiza query http do cliente
	private Response GetResponse() {
		while(client.GetStatus() == HttpClient.Status.Requesting) {
			client.Poll();
            GD.Print("Requesting..."); 
        	OS.DelayMsec(250);
		}
		Debug.Assert(client.GetStatus() == HttpClient.Status.Body
				  || client.GetStatus() == HttpClient.Status.Connected);

		List<byte> rb = new List<byte>(); // Lista que armazenará os dados

		// Enquanto houver dados a serem lidos
		while (client.GetStatus() == HttpClient.Status.Body) {
			client.Poll();
			byte[] chunk = client.ReadResponseBodyChunk(); // Lê um chunk.
			if (chunk.Length == 0) {
				OS.DelayMsec(500);
			} else {
				rb.AddRange(chunk);
			}
		}

		return new Response(Encoding.Latin1.GetString(rb.ToArray()), client.GetResponseCode());
	}

	// Abstração da query de Post
	public Response Post(Godot.Collections.Dictionary input, string[] headers, string url) {
		string query = client.QueryStringFromDict(input);
		Error err = client.Request(HttpClient.Method.Post, url, headers, query);
		if(err == Error.Ok) return GetResponse();
		else return new Response("", -1);
	}

	// Abstração da query de Get 
	public Response Get(string[] headers, string url) {
		Error err = client.Request(HttpClient.Method.Get, url, headers);
		if(err == Error.Ok) return GetResponse();
		else return new Response("", -1);
	}

	// Método que conecta ao portal
	public void Connect(string user, string password) {
		Error err = client.ConnectToHost(UFRGS_URL);
		Debug.Assert(err == Error.Ok);

		// Realiza a conexão
		while (client.GetStatus() == HttpClient.Status.Connecting
		    || client.GetStatus() == HttpClient.Status.Resolving) {

            client.Poll();
            GD.Print("Connecting...");
            OS.DelayMsec(250);

        }
		Debug.Assert(client.GetStatus() == HttpClient.Status.Connected);

		/*
		 *	As variáveis comentadas fazer parte do post normal realizado por um login no portal junto com
		 *	"Destino". Como elas não são necessárias para realizar o login, foram comentadas e não foram
		 *	deletas para caso venham a ser úteis no futuro.
		 */	
		var loginInfo = new Godot.Collections.Dictionary {/*{"Origem", "-"},
														  {"Var1", ""},
														  {"Var2", ""},*/
														  {"usuario", user},
														  {"senha", password},
														  /*{"login", "Enviar"}*/};
		string[] headers = {
			"Content-Type: application/x-www-form-urlencoded",
			"User-Agent: GodotHttpClient/1.0"
		};
		Post(loginInfo, headers, PORTAL);

		// Pega cookie necessário para realizar demais conexões
		string cookie = "";		
		foreach(string header in client.GetResponseHeaders()) {
			if (header.StartsWith("Set-Cookie: PHPSESSID=")) {
				int semicolonIndex = header.IndexOf(';');
				if (semicolonIndex != -1) {
					cookie = header.Substring("Set-Cookie: ".Length, semicolonIndex - "Set-Cookie: ".Length);
				} else {
					cookie = header.Substring("Set-Cookie: ".Length);
				}
				break;
			}
		}
		
		string[] cookieHeaders = {$"Cookie: {cookie}"};

		// Testa se conexão foi bem sucedida
		Response response = Get(cookieHeaders, TEST);

		// O código é 302 pois se a tentativa de login falhar ele retorna 200 com uma mensagem de erro
		// Já se for bem sucedida, ele tenta te redirecionar a outra página (302).
		if(response.code == 302) {
			HttpClientSingleton.cookies = cookieHeaders;
			EmitSignal(SignalName.Connected);	
		} else {
			EmitSignal(SignalName.ConnectionError);	
		}
		

	}

	public static UfrgsConnector GetInstance() { return new UfrgsConnector(); }

}
