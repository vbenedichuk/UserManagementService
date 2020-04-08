using System.Collections.Generic;

namespace UserManagementService.Models.UI.Responses
{
    /// <summary>
    /// Base class for API response.
    /// </summary>
    public class UiResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public UiResponse()
        {
            Messages = new List<UiResponseMessage>();
        }

        public UiResponse(bool success, string code, string message)
        {
            Success = success;
            Messages = new List<UiResponseMessage>(new[] { new UiResponseMessage { Code = code, Message = message } });
        }


        public UiResponse(bool success, List<UiResponseMessage> messages)
        {
            Success = success;
            Messages = new List<UiResponseMessage>(messages);
        }

        /// <summary>
        /// Is request was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Error messages if any.
        /// </summary>
        public List<UiResponseMessage> Messages { get; set; }
    }
}
