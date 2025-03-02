using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectLib
{
    public class StatusCode
    {
        public static StatusCode ConnectDone {  get; } = new StatusCode(201, "Connect Succes");
        public static StatusCode OK {  get; } = new StatusCode(200, "The command was executed successfully");
        public static StatusCode ManyPlayers {  get; } = new StatusCode(501, "Server not have ebought slots for new palyer");
        public static StatusCode IncorrectCommand { get; } = new StatusCode(404, "Command Not Found");
        public static StatusCode NotYourMove { get; } = new StatusCode(406, "It is not your turn to move");
        public static StatusCode BanWord { get; } = new StatusCode(407, "This word is on the ban list");
        public static StatusCode IncorrectArgument { get; } = new StatusCode(408, "Incorrect argument for this command");
        public static StatusCode IncorrectMove { get; } = new StatusCode(409, "Incorrect position for move");



        public int ErrorCode {  get; private set; }
        public string Description { get; private set; } = "";

        public IReadOnlyList<StatusCode> All { get; private set; } = [

            ConnectDone, OK, ManyPlayers, IncorrectCommand, NotYourMove, BanWord, IncorrectArgument, IncorrectMove
            ];

        private StatusCode(int errorCode, string description)
        {
            ErrorCode = errorCode;
            Description = description;
        }
    }
}
