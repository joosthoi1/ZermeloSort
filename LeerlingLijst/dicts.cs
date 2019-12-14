using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeerlingLijst
{
    class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string school { get; set; }

    }
    class UResponse
    {
        public int status { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public int eventId { get; set; }
        public int startRow { get; set; }
        public int endRow { get; set; }
        public int totalRows { get; set; }
        public List<UData> data { get; set; }
    }
    class UData
    {
        public string code { get; set; }
        public string firstName { get; set; }
        public string prefix { get; set; }
        public string lastName { get; set; }
    }
    class UserResponse
    {
        public UResponse response { get; set; }
    }


    class SData
    {
        public List<string> subjects { get; set; }
        public List<string> groups { get; set; }
    }
    class SResponse
    {
        public int status { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public int eventId { get; set; }
        public int startRow { get; set; }
        public int endRow { get; set; }
        public int totalRows { get; set; }
        public List<SData> data { get; set; }
    }
    class ScheduleResponse
    {
        public SResponse response { get; set; }
    }


}
