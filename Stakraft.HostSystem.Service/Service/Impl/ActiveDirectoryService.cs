using Newtonsoft.Json;
using Stakraft.HostSystem.Repository.Repository;
using Stakraft.HostSystem.Service.ServiceDto.ActiveDirectory;
using Stakraft.HostSystem.Service.ServiceDto.Parametros;
using Stakraft.HostSystem.Support.soporte;
using Stakraft.HostSystem.Support.SoporteUtil;
using System.DirectoryServices;

namespace Stakraft.HostSystem.Service.Service.Impl
{
    public class ActiveDirectoryService : IActiveDirectoryService
    {
        private readonly IParametrosRepository parametrosRepository;

        public ActiveDirectoryService(IParametrosRepository parametrosRepository)
        {
            this.parametrosRepository = parametrosRepository;
        }
        public string getUsuarioAd(string usuario)
        {
            var confAd = ObtenerConfiguracionActiveDirectory();
            DirectoryEntry adSearchRoot = new DirectoryEntry(confAd.Ldap, confAd.User, confAd.Password, AuthenticationTypes.Secure);
            DirectorySearcher adSearcher = new DirectorySearcher(adSearchRoot);
            adSearcher.Filter = "(&(objectClass=user)(objectCategory=person))";
            adSearcher.PropertiesToLoad.Add("samaccountname");
            SearchResult result;
            SearchResultCollection iResult = adSearcher.FindAll();
            string usuarioAd = null;
            if (iResult != null)
            {
                for (int counter = 0; counter < iResult.Count; counter++)
                {
                    result = iResult[counter];
                    if (result.Properties.Contains("samaccountname"))
                    {
                        var userName = (String)result.Properties["samaccountname"][0];
                        if (usuario.Contains(userName))
                        {
                            usuarioAd = userName;
                            break;
                        }
                    }
                }
            }
            adSearcher.Dispose();
            adSearchRoot.Dispose();
            return usuarioAd;
        }

        public List<UserAd> ListarUsuarioAd()
        {
            var confAd = ObtenerConfiguracionActiveDirectory();
            DirectoryEntry adSearchRoot = new DirectoryEntry(confAd.Ldap, confAd.User, confAd.Password, AuthenticationTypes.Secure);
            DirectorySearcher adSearcher = new DirectorySearcher(adSearchRoot);
            adSearcher.Filter = "(&(objectClass=user)(objectCategory=person))";
            adSearcher.PropertiesToLoad.Add("samaccountname");
            adSearcher.PropertiesToLoad.Add("displayname");
            adSearcher.PropertiesToLoad.Add("mail");
            adSearcher.PropertiesToLoad.Add("usergroup");
            SearchResult result;
            SearchResultCollection iResult = adSearcher.FindAll();
            List<UserAd> rst = new List<UserAd>();
            if (iResult != null)
            {
                for (int counter = 0; counter < iResult.Count; counter++)
                {
                    result = iResult[counter];
                    if (result.Properties.Contains("samaccountname"))
                    {
                        var item = new UserAd();
                        item.UserName = (String)result.Properties["samaccountname"][0];
                        GetDisplayName(result, item);
                        GetEmail(result, item);
                        GetUserGroup(result, item);
                        rst.Add(item);
                    }
                }
            }
            adSearcher.Dispose();
            adSearchRoot.Dispose();
            return rst;
        }

        private static void GetUserGroup(SearchResult result, UserAd item)
        {
            if (result.Properties.Contains("usergroup"))
            {
                item.Usergroup = (String)result.Properties["usergroup"][0];
            }
        }

        private static void GetEmail(SearchResult result, UserAd item)
        {
            if (result.Properties.Contains("mail"))
            {
                item.Email = (String)result.Properties["mail"][0];
            }
        }

        private static void GetDisplayName(SearchResult result, UserAd item)
        {
            if (result.Properties.Contains("displayname"))
            {
                item.DisplayName = (String)result.Properties["displayname"][0];
            }
        }

        public AdConfiguracionDto ObtenerConfiguracionActiveDirectory()
        {
            var adConfiguracion = this.parametrosRepository.ObtenerParametro("AD_CONFIGURACION");
            if (adConfiguracion == null)
            {
                throw new StatkraftException("No se encontro información sobre el directorio de usuarios");
            }
            var valor = UtilCifrado.Desencripta(adConfiguracion.ValorParametro);
            var sftpParams = JsonConvert.DeserializeObject<AdConfiguracionDto>(valor);
            return sftpParams;
        }
    }
}
