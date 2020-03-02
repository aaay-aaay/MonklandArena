﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkArena {
    public class MonkArena : Partiality.Modloader.PartialityMod {

        #region Versioning
        public string MajorVersion { get; } = "0";
        public string MinorVersion { get; } = "0";
        public string Revision { get; } = "1";
        #endregion

        public MonkArena() {
            author = "Little Tiny Big";
            ModID = "MonkArena";
            Version = $"{MajorVersion}.{MinorVersion}.{Revision}";
        }

        public override void OnEnable() {
            base.OnEnable();
            On.Player.Update += TestPlayerUpdateHook;
        }

        private void TestPlayerUpdateHook(On.Player.orig_Update orig, Player self, bool eu) {
            Logger.LogInfo("Ticked in player");
            orig(self, eu);
        }

        public override void OnDisable() {
            base.OnDisable();
        }
    }
}
