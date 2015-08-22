namespace Battleships.Client
{
    using System;
    using System.Text;

    public class AvailableGameDTO
    {
        public string Id { get; set; }

        public string PlayerOne { get; set; }

        public string State { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("Game: {0}{1}", this.Id, Environment.NewLine);
            stringBuilder.AppendFormat("\tPlayer one: {0}{1}", this.PlayerOne, Environment.NewLine);
            stringBuilder.AppendFormat("\tGame state: {0}", this.State);

            return stringBuilder.ToString();
        }
    }
}
