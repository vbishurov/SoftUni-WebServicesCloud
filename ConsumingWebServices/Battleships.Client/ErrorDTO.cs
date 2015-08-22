namespace Battleships.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json.Linq;

    public class ErrorDTO
    {
        public string Error { get; set; }

        public string Error_Description { get; set; }

        public string Message { get; set; }

        public object ModelState { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.Error_Description))
            {
                return this.Error_Description;
            }

            if (!string.IsNullOrEmpty(this.Message))
            {
                return this.Message;
            }

            var errors = ((JObject)this.ModelState).ToObject<Dictionary<string, IList<string>>>();
            var errorMessages = errors.First().Value[0].Split(new[] { "." }, 2, StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim());
            return string.Format("{0}{1}{2}{3}", this.Message, Environment.NewLine, "\t", string.Join(Environment.NewLine + "\t", errorMessages));
        }
    }
}
