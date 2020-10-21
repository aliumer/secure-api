using PtcApi.Model;
using System.Collections.Generic;

public class AppUserAuth {

    public AppUserAuth() {
        UserName = "Not Authorized";
        BearerToken = "";
    }

    public string UserName { get; set; }
    public string BearerToken { get; set; }
    public bool IsAuthenticated { get; set; }
    public List<AppUserClaim> Claims { get; set; }

}
