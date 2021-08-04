using QLK_API_Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace QLK_API_Client.ViewModel
{
    public class MainViewModel
    {
        public MainViewModel()
        {

        }
        private ObservableCollection<Input> _InputList;
        public ObservableCollection<Input> InputList { get => _InputList; set => _InputList = value; }
        
        public void CallInputAPI()
        {
            ProviderAPI provider = new ProviderAPI();
            string urlApi = "http://localhost:50151/api/Input";
            string method = "HttpGet";
            Dictionary<string, string> i_ParaExtendURL = new Dictionary<string, string>() { };
            Dictionary<string, string> i_HeadersPlus = new Dictionary<string, string>() { };
            Dictionary<string, string> i_Parameter = new Dictionary<string, string>() { };
            string i_BodyJson = "";
            var getInput = provider.CallService(urlApi, method, i_ParaExtendURL, i_HeadersPlus, i_Parameter, i_BodyJson);
            
            
            //Input InputList = JsonConvert.DeserializeObject<LitInput>(getInput);

            var model = JsonConvert.DeserializeObject<ObservableCollection<Input>>(getInput);
            InputList = model;

        }
    }
}
