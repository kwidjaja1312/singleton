/*
 * Copyright 2019 VMware, Inc.
 * SPDX-License-Identifier: EPL-2.0
 */

using Newtonsoft.Json.Linq;

namespace SingletonClient.Implementation
{
    public interface ISingletonComponent
    {
        string GetString(string key);
        bool CheckStatus();
    }

    public class SingletonComponent : ISingletonComponent, ISingletonAccessRemote
    {
        private ISingletonRelease _releaseObject;
        private IComponentMessages _componentCache;

        private string _locale;
        private string _component;

        private SingletonAccessRemoteTask _task;

        public SingletonComponent(ISingletonRelease releaseObject, string locale, string component)
        {
            _releaseObject = releaseObject;
            _locale = locale;
            _component = component;

            IConfig config = releaseObject.GetRelease().GetConfig();
            int interval = config.GetIntValue(ConfigConst.KeyInterval);
            int tryDelay = config.GetIntValue(ConfigConst.KeyTryDelay);

            _task = new SingletonAccessRemoteTask(this, interval, tryDelay);

            ICacheMessages productCache = releaseObject.GetProductMessages();
            ILanguageMessages langCache = productCache.GetLanguageMessages(locale);
            _componentCache = langCache.GetComponentMessages(component);
        }

        public void GetDataFromRemote()
        {
            string adr = _releaseObject.GetApi().GetComponentApi(_component, _locale);
            JObject obj = SingletonUtil.HttpGetJson(_releaseObject.GetAccessService(), adr);
            if (SingletonUtil.CheckResponseValid(obj))
            {
                JObject result = obj.Value<JObject>(SingletonConst.KeyResult);
                JObject data = result.Value<JObject>(SingletonConst.KeyData);
                JObject messages = data.Value<JObject>(SingletonConst.KeyMessages);

                foreach (var item in messages)
                {
                    _componentCache.SetString(item.Key.ToString(), item.Value.ToString());
                }
            }
        }

        public int GetDataCount()
        {
            return _componentCache.GetCount();
        }

        public bool CheckStatus()
        {
            return _task.CheckStatus();
        }

        public string GetString(string key)
        {
            string message = _componentCache.GetString(key);
            return message;
        }
    }
}


