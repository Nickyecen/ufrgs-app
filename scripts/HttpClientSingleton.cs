using Godot;
using System;

// Singleton do Cliente HTTP para que o mesmo possa ser usado durante toda a utilização do aplicativo
public partial class HttpClientSingleton : Node {

	public HttpClient client { get; private set; }
	public static string[] cookies {get; set;}
	private static HttpClientSingleton _instance;

	public override void _Ready() {
		if(client == null) {
			client = new HttpClient();
		}
	}

	public static HttpClientSingleton GetInstance() {
		if (_instance == null) {
			_instance = new HttpClientSingleton();
		} if(_instance.client == null) {
			_instance.client = new HttpClient();
		}

		return _instance;
	}

	public override void _ExitTree() {
		client?.Dispose();
	}

}
