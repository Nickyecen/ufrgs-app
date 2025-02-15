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

	public List<Ticket> availableTickets;
	public List<Ticket> usedTickets;

	private const string TICKETS_URL = "/RU/tru/";

	public override void _Ready() {
		// Busca tíquetes na página
		Response ticketResponse = GetNode<UfrgsConnector>("UfrgsConnector").Get(HttpClientSingleton.cookies, TICKETS_URL);
		Debug.Assert(ticketResponse.code != -1);

		// Extrai tíquetes do html
		var doc = new HtmlDocument();
		doc.LoadHtml(ticketResponse.text);
		HtmlNode ticketTable = GetTicketTable(doc);
		availableTickets = GetAvailableTickets(ticketTable);
		usedTickets = GetUsedTickets(ticketTable);

		// Adiciona tíquetes à tabela na interface
		AddTickets();
	}

	public void OnBackPressed() {
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
	private void AddTickets() {

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
