﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GabberPCL.Resources {
    using System;
    using System.Reflection;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class StringResources {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal StringResources() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("GabberPCL.Resources.StringResources", typeof(StringResources).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        public static string common_ui_forms_email_label {
            get {
                return ResourceManager.GetString("common.ui.forms.email.label", resourceCulture);
            }
        }
        
        public static string common_ui_forms_password_label {
            get {
                return ResourceManager.GetString("common.ui.forms.password.label", resourceCulture);
            }
        }
        
        public static string common_ui_forms_email_validate_empty {
            get {
                return ResourceManager.GetString("common.ui.forms.email.validate.empty", resourceCulture);
            }
        }
        
        public static string common_ui_forms_email_validate_invalid {
            get {
                return ResourceManager.GetString("common.ui.forms.email.validate.invalid", resourceCulture);
            }
        }
        
        public static string common_ui_forms_password_validate_empty {
            get {
                return ResourceManager.GetString("common.ui.forms.password.validate.empty", resourceCulture);
            }
        }
        
        public static string login_api_error_GENERAL {
            get {
                return ResourceManager.GetString("login.api.error.GENERAL", resourceCulture);
            }
        }
        
        public static string login_api_error_NO_INTERNET {
            get {
                return ResourceManager.GetString("login.api.error.NO_INTERNET", resourceCulture);
            }
        }
        
        public static string login_api_error_AUTH_INVALID_PASSWORD {
            get {
                return ResourceManager.GetString("login.api.error.AUTH_INVALID_PASSWORD", resourceCulture);
            }
        }
        
        public static string login_api_error_AUTH_USER_404 {
            get {
                return ResourceManager.GetString("login.api.error.AUTH_USER_404", resourceCulture);
            }
        }
        
        public static string login_ui_submit_button {
            get {
                return ResourceManager.GetString("login.ui.submit.button", resourceCulture);
            }
        }
        
        public static string register_api_error_AUTH_INVALID_PASSWORD {
            get {
                return ResourceManager.GetString("register.api.error.AUTH_INVALID_PASSWORD", resourceCulture);
            }
        }
        
        public static string register_api_error_AUTH_USER_404 {
            get {
                return ResourceManager.GetString("register.api.error.AUTH_USER_404", resourceCulture);
            }
        }
        
        public static string register_ui_fullname_label {
            get {
                return ResourceManager.GetString("register.ui.fullname.label", resourceCulture);
            }
        }
        
        public static string register_ui_submit_button {
            get {
                return ResourceManager.GetString("register.ui.submit.button", resourceCulture);
            }
        }
        
        public static string register_ui_fullname_validate_empty {
            get {
                return ResourceManager.GetString("register.ui.fullname.validate.empty", resourceCulture);
            }
        }
    }
}
