using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ChatApp.Shared.DTOs;
using ChatApp.Wpf.Services;

namespace ChatApp.Wpf;

public partial class MainWindow : Window
{
	private readonly ChatApiClient _apiClient = new();

	private readonly DispatcherTimer _pollingTimer = new();

	private UserResponse? _currentUser;

	private int? _lastMessageId;

	public MainWindow()
	{
		InitializeComponent();

		_pollingTimer.Interval = TimeSpan.FromSeconds(1);
		_pollingTimer.Tick += PollingTimer_Tick;
	}

	private async void LoginButton_Click(object sender, RoutedEventArgs e)
	{
		var username = UsernameTextBox.Text.Trim();

		if (string.IsNullOrWhiteSpace(username))
		{
			MessageBox.Show("Zadaj meno používateľa.");
			return;
		}

		try
		{
			SetUiEnabled(false);
			StatusTextBlock.Text = "Connecting...";

			_currentUser = await _apiClient.CreateOrGetUserAsync(username);

			UsernameTextBox.IsEnabled = false;
			LoginButton.IsEnabled = false;
			MessageTextBox.IsEnabled = true;
			SendButton.IsEnabled = true;

			StatusTextBlock.Text = $"Connected as {_currentUser.Username}";

			await LoadInitialMessagesAsync();

			_pollingTimer.Start();

			MessageTextBox.Focus();
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Nepodarilo sa pripojiť k API: {ex.Message}");
			StatusTextBlock.Text = "Connection failed";

			UsernameTextBox.IsEnabled = true;
			LoginButton.IsEnabled = true;
		}
	}

	private async Task LoadInitialMessagesAsync()
	{
		var messages = await _apiClient.GetMessagesAsync();

		MessagesListBox.Items.Clear();

		foreach (var message in messages)
		{
			AddMessageToList(message);
		}

		_lastMessageId = messages.LastOrDefault()?.Id;
	}

	private async void PollingTimer_Tick(object? sender, EventArgs e)
	{
		try
		{
			var newMessages = await _apiClient.GetMessagesAsync(_lastMessageId);

			foreach (var message in newMessages)
			{
				AddMessageToList(message);
				_lastMessageId = message.Id;
			}
		}
		catch
		{
			StatusTextBlock.Text = "API unavailable...";
		}
	}

	private async void SendButton_Click(object sender, RoutedEventArgs e)
	{
		await SendMessageAsync();
	}

	private async void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Enter)
		{
			await SendMessageAsync();
		}
	}

	private async Task SendMessageAsync()
	{
		if (_currentUser is null)
		{
			MessageBox.Show("Najprv sa pripoj ako používateľ.");
			return;
		}

		var text = MessageTextBox.Text.Trim();

		if (string.IsNullOrWhiteSpace(text))
		{
			return;
		}

		try
		{
			SendButton.IsEnabled = false;

			var sentMessage = await _apiClient.SendMessageAsync(_currentUser.Id, text);

			MessageTextBox.Clear();

			// Aby sa vlastná správa zobrazila hneď, nečakáme na ďalší polling tick.
			if (!_lastMessageId.HasValue || sentMessage.Id > _lastMessageId.Value)
			{
				AddMessageToList(sentMessage);
				_lastMessageId = sentMessage.Id;
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Správu sa nepodarilo odoslať: {ex.Message}");
		}
		finally
		{
			SendButton.IsEnabled = true;
			MessageTextBox.Focus();
		}
	}

	private void AddMessageToList(MessageResponse message)
	{
		var localTime = message.CreatedAt.ToLocalTime().ToString("HH:mm:ss");

		var formattedMessage = $"[{localTime}] {message.Username}: {message.Text}";

		MessagesListBox.Items.Add(formattedMessage);
		MessagesListBox.ScrollIntoView(formattedMessage);
	}

	private void SetUiEnabled(bool isEnabled)
	{
		UsernameTextBox.IsEnabled = isEnabled;
		LoginButton.IsEnabled = isEnabled;
		MessageTextBox.IsEnabled = isEnabled && _currentUser is not null;
		SendButton.IsEnabled = isEnabled && _currentUser is not null;
	}
}