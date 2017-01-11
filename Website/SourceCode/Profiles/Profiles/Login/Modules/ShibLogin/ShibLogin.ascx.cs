﻿/*  
 
    Copyright (c) 2008-2012 by the President and Fellows of Harvard College. All rights reserved.  
    Profiles Research Networking Software was developed under the supervision of Griffin M Weber, MD, PhD.,
    and Harvard Catalyst: The Harvard Clinical and Translational Science Center, with support from the 
    National Center for Research Resources and Harvard University.


    Code licensed under a BSD License. 
    For details, see: LICENSE.txt 
  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web.UI.HtmlControls;
using System.Configuration;

using Profiles.Login.Utilities;
using Profiles.Framework.Utilities;

namespace Profiles.Login.Modules.ShibLogin
{
    public partial class ShibLogin : System.Web.UI.UserControl
    {
        Framework.Utilities.SessionManagement sm;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                if (Request.QueryString["method"].ToString() == "logout")
                {

                    sm.SessionLogout();
                    sm.SessionDestroy();
                    Response.Redirect(Request.QueryString["redirectto"].ToString());
                }
                else if (Request.QueryString["method"].ToString() == "shibboleth")
                {
                    // added by Eric
                    // If they specify an Idp, then check that they logged in from the configured IDP
                    bool authenticated = false;
                    if (ConfigurationManager.AppSettings["Shibboleth.ShibIdentityProvider"] == null ||
                        ConfigurationManager.AppSettings["Shibboleth.ShibIdentityProvider"].ToString().Equals(Request.Headers.Get("ShibIdentityProvider").ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        //String userName = Request.Headers.Get(ConfigurationManager.AppSettings["Shibboleth.UserNameHeader"].ToString()); //"025693078";
                        String userName = Request[ConfigurationManager.AppSettings["Shibboleth.UserNameHeader"]].ToString(); //"025693078";

                        if (userName != null && userName.Trim().Length > 0)
                        {
                            Profiles.Login.Utilities.DataIO data = new Profiles.Login.Utilities.DataIO();
                            Profiles.Login.Utilities.User user = new Profiles.Login.Utilities.User();

                            user.UserName = userName;
                            if (data.UserLoginExternal(ref user))
                            {
                                authenticated = true;
                                RedirectAuthenticatedUser();
                            }
                        }
                    }
                    if (!authenticated)
                    {
                        Framework.Utilities.DebugLogging.Log("Ketan - notauthenticated");
                        // try and just put their name in the session.
                        //sm.Session().ShortDisplayName = Request.Headers.Get("ShibdisplayName");
                        RedirectAuthenticatedUser();
                    }
                }
                else if (Request.QueryString["method"].ToString() == "login")
                {
                    // see if they already have a login session, if so don't send them to shibboleth
                    Profiles.Framework.Utilities.SessionManagement sm = new Profiles.Framework.Utilities.SessionManagement();
                    String viewerId = sm.Session().PersonURI;
                    if (viewerId != null && viewerId.Trim().Length > 0)
                    {
                        RedirectAuthenticatedUser();
                    }
                    else if (!String.IsNullOrEmpty(Request[ConfigurationManager.AppSettings["Shibboleth.SessionID"].ToString()].ToString()))


                    {
                        bool authenticated = false;

                        String userName = Request[ConfigurationManager.AppSettings["Shibboleth.UserNameHeader"]].ToString(); //"025693078";
                        if (userName != null && userName.Trim().Length > 0)
                        {
                            Framework.Utilities.DebugLogging.Log("Ketan - username " + userName);
                            Profiles.Login.Utilities.DataIO data = new Profiles.Login.Utilities.DataIO();
                            Profiles.Login.Utilities.User user = new Profiles.Login.Utilities.User();

                            user.UserName = userName;
                            if (data.UserLoginExternal(ref user))
                            {
                                Framework.Utilities.DebugLogging.Log("Ketan - data.userloginexternal " + userName);
                                authenticated = true;
                                RedirectAuthenticatedUser();
                            }
                        }








                        }

                        else
                    {
                        string redirect = Root.Domain + "/login/default.aspx?method=shibboleth";
                        if (Request.QueryString["redirectto"] == null && Request.QueryString["edit"] == "true")
                            redirect += "&edit=true";
                        
                        else
                            redirect += "&redirectto=" + Request.QueryString["redirectto"].ToString();
                            redirect += "&target=" + Request.QueryString["redirectto"].ToString();
                        Response.Redirect(ConfigurationManager.AppSettings["Shibboleth.LoginURL"].ToString().Trim() +
                            HttpUtility.UrlEncode(redirect));
                    }
                }

            }


        }

        private void RedirectAuthenticatedUser()
        {

            Framework.Utilities.DebugLogging.Log("Ketan - redirectto " + Request.QueryString["redirectto"]);
            Framework.Utilities.DebugLogging.Log("Ketan - edit " + Request.QueryString["edit"]);

            if (Request.QueryString["redirectto"] == null && Request.QueryString["edit"] == "true")
            {
                Framework.Utilities.DebugLogging.Log("Ketan - redirecauser 1 ");
                Response.Redirect(Root.Domain + "/edit/" + sm.Session().NodeID);
            }
            else if (Request.QueryString["redirectto"] != null)

            {
                if ("mypage".Equals(Request.QueryString["redirectto"].ToLower())) 
                {
                    Framework.Utilities.DebugLogging.Log("Ketan - redirecauser 2 ");
                    Response.Redirect(Root.Domain + "/profile/" + sm.Session().NodeID);
                }
                else if ("myproxies".Equals(Request.QueryString["redirectto"].ToLower()))
                {
                    Framework.Utilities.DebugLogging.Log("Ketan - redirecauser 3 ");
                    Response.Redirect(Root.Domain + "/proxy/default.aspx?subject=" + sm.Session().NodeID);
                }
                else 
                {
                    Framework.Utilities.DebugLogging.Log("Ketan - redirecauser 4 ");
                    Response.Redirect(Request.QueryString["redirectto"].ToString());
                }
            }
            Framework.Utilities.DebugLogging.Log("Ketan - redirecauser 5 ");
            Response.Redirect(Root.Domain);
        }

        public ShibLogin() { }
        public ShibLogin(XmlDocument pagedata, List<ModuleParams> moduleparams, XmlNamespaceManager pagenamespaces)
        {
            Framework.Utilities.DebugLogging.Log("Ketan - session management 1");
            Framework.Utilities.DebugLogging.Log("Ketan - session management 1" + HttpContext.Current.Session["PROFILES_SESSION"].ToString());
            sm = new Profiles.Framework.Utilities.SessionManagement();
            Framework.Utilities.DebugLogging.Log("Ketan - session management 2" + sm.ToString());
            Framework.Utilities.DebugLogging.Log("Ketan - session management 2" + HttpContext.Current.Session["PROFILES_SESSION"].ToString());
        }

    }
}