namespace Battleships.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    public class GameEngine
    {
        private readonly HttpClient client = new HttpClient();

        # region EndPoints

        private const string ServiceUrl = "http://localhost:62858/";

        private const string RegisterEndPoint = ServiceUrl + "api/account/register";
        private const string LoginEndPoint = ServiceUrl + "token";
        private const string CreateGameEndPoint = ServiceUrl + "api/games/create";
        private const string JoinGameEndPoint = ServiceUrl + "api/games/join";
        private const string PlayEndPoint = ServiceUrl + "api/games/play";
        private const string GetAvailableGamesEndPoint = ServiceUrl + "api/games/available";

        # endregion

        public void Run()
        {
            string line;

            while (true)
            {
                line = Console.ReadLine();

                if (line == "$ end")
                {
                    break;
                }

                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var lineChunks = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                if (lineChunks.Length < 2 || lineChunks[0] != "$")
                {
                    continue;
                }

                string command = lineChunks[1];
                string[] commandArguments = lineChunks.Skip(2).ToArray();

                this.ExecuteCommand(command, commandArguments);
            }

            Console.WriteLine("Exiting game");
        }


        private void ExecuteCommand(string command, string[] commandParams)
        {
            switch (command)
            {
                case "register":
                    {
                        this.Register(commandParams);
                        break;
                    }
                case "login":
                    {
                        this.Login(commandParams);
                        break;
                    }
                case "create-game":
                    {
                        this.CreateGame();
                        break;
                    }
                case "join-game":
                    {
                        this.JoinGame(commandParams);
                        break;
                    }
                case "play":
                    {
                        this.Play(commandParams);
                        break;
                    }
                case "list-games":
                    {
                        this.ListGames();
                        break;
                    }
                default: throw new NotSupportedException(String.Format("Command {0} is not supported", command));
            }
        }

        private async void Play(string[] playParams)
        {
            if (playParams.Length < 3)
            {
                Console.WriteLine("Please specify game id, x position and y position");
            }

            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("GameId", playParams[0]),
                new KeyValuePair<string, string>("PositionX", playParams[1]),
                new KeyValuePair<string, string>("PositionY", playParams[2])
            });

            var response = await this.client.PostAsync(PlayEndPoint, formData);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsAsync<ErrorDTO>();
                Console.WriteLine(error);
                return;
            }

            Console.WriteLine("Move played successfully. Next player's turn.");
        }

        private async void ListGames()
        {
            var response = await this.client.GetAsync(GetAvailableGamesEndPoint);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsAsync<ErrorDTO>();
                Console.WriteLine(error);
                return;
            }

            var games = await response.Content.ReadAsAsync<IEnumerable<AvailableGameDTO>>();

            Console.WriteLine(string.Join(Environment.NewLine, games));
        }

        private async void JoinGame(string[] joinGameParams)
        {
            if (joinGameParams.Length < 1)
            {
                Console.WriteLine("Please supply game id.");

                return;
            }

            var gameId = joinGameParams[0].Replace("\"", "");

            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("GameId", gameId), 
            });

            var response = await this.client.PostAsync(JoinGameEndPoint, formData);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsAsync<ErrorDTO>();
                Console.WriteLine(error);
                return;
            }

            Console.WriteLine("Game {0} joined successfully", gameId);
        }

        private async void CreateGame()
        {
            var response = await this.client.PostAsync(CreateGameEndPoint, null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsAsync<ErrorDTO>();
                Console.WriteLine(error);
                return;
            }

            var gameId = await response.Content.ReadAsStringAsync();

            Console.WriteLine(gameId);
        }

        private async void Login(string[] loginParams)
        {
            if (loginParams.Length < 2)
            {
                Console.WriteLine("Username or password is incorrect");

                return;
            }

            var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Username", loginParams[0]),
                    new KeyValuePair<string, string>("Password", loginParams[1]),
                    new KeyValuePair<string, string>("grant_type", "password")
                });

            var response = await this.client.PostAsync(LoginEndPoint, formData);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsAsync<ErrorDTO>();
                Console.WriteLine(error);
                return;
            }

            var result = await response.Content.ReadAsAsync<LoginDTO>();

            this.client.DefaultRequestHeaders.Add("Authorization", result.ToString());

            Console.WriteLine("User {0} logged in successfully", loginParams[0]);
        }

        private async void Register(params string[] registerParams)
        {
            if (registerParams.Length < 3)
            {
                Console.WriteLine("Email, Password and ConfirmPassword are required");

                return;
            }

            Regex emailRegex = new Regex(@"^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$");

            if (!emailRegex.IsMatch(registerParams[0]))
            {
                Console.WriteLine("Email is invalid.");
                return;
            }

            if (registerParams[1] != registerParams[2])
            {
                Console.WriteLine("Passwords do not match");
                return;
            }

            var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Email", registerParams[0]),
                    new KeyValuePair<string, string>("Password", registerParams[1]),
                    new KeyValuePair<string, string>("ConfirmPassword", registerParams[2])
                });

            var response = await this.client.PostAsync(RegisterEndPoint, formData);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsAsync<ErrorDTO>();
                Console.WriteLine(error);
                return;
            }

            Console.WriteLine("User {0} registered successfully", registerParams[0]);
        }
    }
}
