namespace Battleships.Client
{
    using System;

    public class LoginDTO
    {
        public string Access_Token { get; set; }

        public string Token_Type { get; set; }

        public int Expires_In { get; set; }

        public string Username { get; set; }

        public DateTime Issued { get; set; }

        public DateTime Expires { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", this.Token_Type, this.Access_Token);
        }
    }
}
