using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Oxide.Core;

namespace Oxide.Plugins
{
	[Info("SkinChanger", "WOLF", "1.0.0")]
	[Description("Change Skins")]

	class SkinChanger : RustPlugin
	{
		private const string useperm = "skinchanger.use";
        private Configuration _config;

        private void Init()
        {
            lang.SetServerLanguage(_config.DefaultLang);

            permission.RegisterPermission(useperm, this);
            cmd.AddChatCommand(_config.command, this, nameof(cmdChangeSkin));
            cmd.AddConsoleCommand(_config.command, this, nameof(cmdChangeSkin));
        }
        void cmdChangeSkin(BasePlayer player, string cmd, string[] args)
        {
			if (!permission.UserHasPermission(player.UserIDString, useperm))
            {
                PrintToChat(player, Lang("Permission"));
                return;
            }
            if(args.Length == 0)
            {
                PrintToChat(player, Lang("Descriptions", null, _config.Prefix, _config.command));
            }

            if(args.Length == 1)
            {
                var item = player.GetActiveItem();
                if (item != null)
                {
                    SetSkin(player, item, args[0]);
                }
            }
            
            if(args.Length == 2)
            {
                var beltpostion = 0;
                if (int.TryParse(args[1], out beltpostion) == false)
                {
                    PrintToChat(player, Lang("Error", null));
                    return;
                }
                var belt = player.inventory.containerBelt.GetSlot(beltpostion);
                if (belt != null)
                {
                    SetBelt(player, belt, args[0], args[1]);
                }
            }
        }
        private void SetSkin(BasePlayer player, Item item, string skinIDString)
        {
            var skinID = 0UL;
            if (ulong.TryParse(skinIDString, out skinID) == false)
            {
                PrintToChat(player, Lang("Error", null));
                return;
            }
            if (skinID == 0)
            {
                return;
            }

            item.skin = skinID;
            item.MarkDirty();

            var held = item.GetHeldEntity();
            if (held != null)
            {
                held.skinID = skinID;
                held.SendNetworkUpdate();
            }
            PrintToChat(player, Lang("Active_Change", null, skinID, item.info.shortname, item.skin));
        }

        private void SetBelt(BasePlayer player, Item item, string skinIDString, string belt)
        {
            var skinID = 0UL;
            if (ulong.TryParse(skinIDString, out skinID) == false)
            {
                PrintToChat(player, Lang("Error", null));
                return;
            }
            var beltpostion = 0;
            if(int.TryParse(belt, out beltpostion) == false)
            {
                PrintToChat(player, Lang("Error", null));
                return;
            }
            if (skinID == 0)
            {
                return;
            }

            item.skin = skinID;
            item.MarkDirty();

            var held = item.GetHeldEntity();
            if (held != null)
            {
                held.skinID = skinID;
                held.SendNetworkUpdate();
            }
            PrintToChat(player, Lang("Belt_Change", null, skinID, beltpostion, item.info.shortname, item.skin));
        }

        #region Lang
        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["Descriptions"] = "{0}\n" +
                "/{1} skinid\n" +
                "(Changed Skin.[ALL])\n" +
                "/{1} skinid 0~5\n" +
                "(Change the skin to position 0 on the belt.[Cloth Only])",
                ["Active_Change"] = "{1} - {2} Skin changed to {0}",
                ["Belt_Change"] = "Belt {1} postion {2} item changed to {0} skin",
                ["Permission"] = "<color=red>Not permission.</color>",
                ["Error"] = "Please enter the correct unique number",
                ["Lang"] = "The default language has been changed to {0}."
            }, this, "en");
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["Descriptions"] = "{0}\n" +
                "/{1} 스킨고유번호\n" +
                "(스킨이 변경됩니다.[전체])\n" +
                "/{1} 스킨고유번호 0\n" +
                "(벨트 0번 위치에 스킨을 변경합니다.[옷전용])",
                ["Active_Change"] = "{0} 으로 스킨이 변경되었습니다.",
                ["Belt_Change"] = "벨트 {1} 위치 {2} 아이템 스킨이 {0} 으로 변경되었습니다.",
                ["Permission"] = "<color=red>당신은 권한이 없습니다.</color>",
                ["Error"] = "정확한 고유번호를 입력하세요.",
                ["Lang"] = "기본 언어가 {0} 로 변경되었습니다."
            }, this, "kr");
        }

        private string Lang(string key, string id = null, params object[] args)
        {
            return string.Format(lang.GetMessage(key, this, id), args);
        }
        #endregion

        #region Config
        protected override void LoadConfig()
        {
            base.LoadConfig();
            _config = Config.ReadObject<Configuration>();
            SaveConfig();
        }

        protected override void LoadDefaultConfig() => _config = new Configuration();

        protected override void SaveConfig() => Config.WriteObject(_config);

        private class Configuration
        {

            [JsonProperty("Prefix")]
            public string Prefix { get; set; } = "<color=#00ffff>[ SkinChanger ]</color> - ";
            [JsonProperty("Default Language Settings")]
            public string DefaultLang { get; set; } = "en";
            [JsonProperty("Command Settings")]
            public string command { get; set; } = "sc";

        }
        #endregion
    }
}
