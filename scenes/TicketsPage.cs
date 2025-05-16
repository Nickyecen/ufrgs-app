using Godot;
using System;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Collections.Generic;

// Utilizado para guardar as informações do tíquete apresentadas no portal
public struct Ticket {
	public bool available;
	public string ticket;
	public string cost;
	public string emissionDate;
	public string emissionAs;
	public string type;
	public string usedDate;
	public string restaurant;
}

// Classe da página que apresenta os tíquetes
public partial class TicketsPage : Control {

	[Signal] public delegate void TicketGatheringErrorEventHandler();

	public List<Ticket> availableTickets;
	public List<Ticket> usedTickets;

	public String lastGatheredTime;

	private const string TICKETS_URL = "/RU/tru/";
	private const string USED_TICKETS_PATH = "user://.cache/usedTickets";
	private const string AVAILABLE_TICKETS_PATH = "user://.cache/availableTickets";
	private const string LAST_GATHER_TICKETS_PATH = "user://.cache/lastGatherTicketTime";

	public override void _Ready() {
		// Carrega os Tíquetes do arquivo cache para acesso offline
		LoadTicketsFromCache();
		//GatherTickets();

		// Adiciona tíquetes à tabela na interface
		UpdateTickets();
	}

	public void LoadTicketsFromCache() {
		var savedUsedTickets = new List<Ticket>();	
		var savedAvailableTickets = new List<Ticket>();	

		using var usedTicketsFile = FileAccess.Open(USED_TICKETS_PATH, FileAccess.ModeFlags.Read);
		while(!usedTicketsFile.EofReached()) {
			Ticket nextTicket;
			nextTicket.available = usedTicketsFile.Get8() == (byte) 1;
			nextTicket.ticket = usedTicketsFile.GetPascalString();
			nextTicket.cost = usedTicketsFile.GetPascalString();
			nextTicket.emissionDate = usedTicketsFile.GetPascalString();
			nextTicket.emissionAs = usedTicketsFile.GetPascalString();
			nextTicket.type = usedTicketsFile.GetPascalString();
			nextTicket.usedDate = usedTicketsFile.GetPascalString();
			nextTicket.restaurant = usedTicketsFile.GetPascalString();
			savedUsedTickets.Add(nextTicket);
			GD.Print("Read ticket used " + nextTicket.ticket);
		}
		usedTicketsFile.Close();

		using var availableTicketsFile = FileAccess.Open(AVAILABLE_TICKETS_PATH, FileAccess.ModeFlags.Read);
		while(!availableTicketsFile.EofReached()) {
			Ticket nextTicket;
			nextTicket.available 		= availableTicketsFile.Get8() == (byte) 1;
			nextTicket.ticket 			= availableTicketsFile.GetPascalString();
			nextTicket.cost 			= availableTicketsFile.GetPascalString();
			nextTicket.emissionDate 	= availableTicketsFile.GetPascalString();
			nextTicket.emissionAs 		= availableTicketsFile.GetPascalString();
			nextTicket.type 			= availableTicketsFile.GetPascalString();
			nextTicket.usedDate 		= availableTicketsFile.GetPascalString();
			nextTicket.restaurant 		= availableTicketsFile.GetPascalString();
			savedAvailableTickets.Add(nextTicket);
			GD.Print("Read ticket available " + nextTicket.ticket);
		}
		availableTicketsFile.Close();

		using var ticketTimeFile = FileAccess.Open(LAST_GATHER_TICKETS_PATH, FileAccess.ModeFlags.Read);
		lastGatheredTime = ticketTimeFile.GetPascalString();
		ticketTimeFile.Close();

		usedTickets = savedUsedTickets;
		availableTickets = savedAvailableTickets;
	}

	public void SaveTicketsToCache() {
		using var usedTicketsFile = FileAccess.Open(USED_TICKETS_PATH, FileAccess.ModeFlags.Write);
		foreach(Ticket ticket in usedTickets) {
			usedTicketsFile.Store8(ticket.available ? (byte) 1 : (byte) 0);
			usedTicketsFile.StorePascalString(ticket.ticket);
			usedTicketsFile.StorePascalString(ticket.cost);
			usedTicketsFile.StorePascalString(ticket.emissionDate);
			usedTicketsFile.StorePascalString(ticket.emissionAs);
			usedTicketsFile.StorePascalString(ticket.type);
			usedTicketsFile.StorePascalString(ticket.usedDate);
			usedTicketsFile.StorePascalString(ticket.restaurant);
		}

		using var availableTicketsFile = FileAccess.Open(AVAILABLE_TICKETS_PATH, FileAccess.ModeFlags.Write);
		foreach(Ticket ticket in availableTickets) {
			availableTicketsFile.Store8(ticket.available ? (byte) 1 : (byte) 0);
			availableTicketsFile.StorePascalString(ticket.ticket);
			availableTicketsFile.StorePascalString(ticket.cost);
			availableTicketsFile.StorePascalString(ticket.emissionDate);
			availableTicketsFile.StorePascalString(ticket.emissionAs);
			availableTicketsFile.StorePascalString(ticket.type);
			availableTicketsFile.StorePascalString(ticket.usedDate);
			availableTicketsFile.StorePascalString(ticket.restaurant);
		}

		using var ticketTimeFile = FileAccess.Open(LAST_GATHER_TICKETS_PATH, FileAccess.ModeFlags.Write);
		ticketTimeFile.StorePascalString(lastGatheredTime);
		ticketTimeFile.Close();

	}

	public void GatherTickets() {
		// Busca tíquetes na página
		Response ticketResponse = GetNode<UfrgsConnector>("UfrgsConnector").Get(HttpClientSingleton.cookies, TICKETS_URL);
		if(ticketResponse.code == -1) {
			EmitSignal(SignalName.TicketGatheringError);
			return;
		}

		// Extrai tíquetes do html
		var doc = new HtmlDocument();
		doc.LoadHtml(ticketResponse.text);
		HtmlNode ticketTable = GetTicketTable(doc);
		availableTickets = GetAvailableTickets(ticketTable);
		usedTickets = GetUsedTickets(ticketTable);

		lastGatheredTime = Time.GetDatetimeStringFromSystem(useSpace: true);
	}

	public void OnBackPressed() {
		SaveTicketsToCache();
		GetTree().ChangeSceneToFile("res://scenes/main_page.tscn");
	}

	// Pega tabela de tíquetes do html
	private HtmlNode GetTicketTable(HtmlDocument htmlDoc) {
		return htmlDoc.DocumentNode.SelectSingleNode("//table");
	}

	// Gera lista de tíquetes disponíveis a partir da tabela de tíquetes
	private List<Ticket> GetAvailableTickets(HtmlNode table) {
		var tickets = new List<Ticket>();

		foreach(var row in table.SelectNodes("//tr")) {
				var cells = row.SelectNodes("td");
				if(cells == null || cells.Count != 4) continue;
				var ticket = new Ticket {
					available = true,
					ticket = cells[0].InnerText.Trim(),
					cost = cells[1].InnerText.Trim(),
					emissionDate = cells[2].InnerText.Trim(),
					emissionAs = cells[3].InnerText.Trim(),
					type = "",
					usedDate = "",
					restaurant = ""
				};
				tickets.Add(ticket);
		}

		return tickets;
	}

	// Gera lista de tíquetes usados a partir da tabela de tíquetes
	private List<Ticket> GetUsedTickets(HtmlNode table) {
		var tickets = new List<Ticket>();

		foreach(var row in table.SelectNodes("//tr")) {
				GD.Print(row.InnerHtml);
				var cells = row.SelectNodes("td");
				if(cells == null || cells.Count != 7) continue;
				var ticket = new Ticket {
					available = false,
					ticket = cells[0].InnerText.Trim(),
					cost = cells[1].InnerText.Trim(),
					type = cells[2].InnerText.Trim(),
					emissionDate = cells[3].InnerText.Trim(),
					emissionAs = cells[4].InnerText.Trim(),
					usedDate = cells[5].InnerText.Trim(),
					restaurant = cells[6].InnerText.Trim()
				};
				tickets.Add(ticket);
		}

		return tickets;
	}

	// Adiciona tíquetes disponíveis e usados à interface
	private void UpdateTickets() {

		GridContainer available = GetNode<GridContainer>("VBoxContainer/MarginContainer/TabContainer/Available/Data");
		GridContainer used = GetNode<GridContainer>("VBoxContainer/MarginContainer/TabContainer/Used/Data");

		// Adiciona tíquetes disponíveis
		foreach(Ticket ticket in availableTickets) {
			Label[] labels = new Label[4];
			for(int i = 0; i < labels.Length; i++) {
				labels[i] = new Label();
				labels[i].SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
				labels[i].HorizontalAlignment = HorizontalAlignment.Center;
			}

			labels[0].Text = ticket.ticket;
			labels[1].Text = ticket.cost;
			labels[2].Text = ticket.emissionDate;
			labels[3].Text = ticket.emissionAs;

			foreach(var label in labels) available.AddChild(label);
		}
	
		// Adiciona tíquetes usados	
		foreach(Ticket ticket in usedTickets) {
			Label[] labels = new Label[7];
			for(int i = 0; i < labels.Length; i++) {
				labels[i] = new Label();
				labels[i].SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
				labels[i].HorizontalAlignment = HorizontalAlignment.Center;
			}

			labels[0].Text = ticket.ticket;
			labels[1].Text = ticket.cost;
			labels[2].Text = ticket.type;
			labels[3].Text = ticket.emissionDate;
			labels[4].Text = ticket.emissionAs;
			labels[5].Text = ticket.usedDate;
			labels[6].Text = ticket.restaurant;

			foreach(var label in labels) used.AddChild(label);
		}	
	}	

}
