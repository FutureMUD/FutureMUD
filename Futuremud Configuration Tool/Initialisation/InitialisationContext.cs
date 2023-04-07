using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Database;
using MudSharp.Framework;

namespace Futuremud_Configuration_Tool.Initialisation {
    public class InitialisationContext : IDisposable {
        public Futuremud Gameworld { get; set; }
        public FMDB DB { get; set; }
        public FME.FuturemudDatabaseContext Context { get; set; }

        public async Task Initialise() {
            await Task.Run(() => {
                Console.WriteLine("Initialising FutureMUD Game...");

                // TODO - this could be loaded from a configuration file
                FMDB.Provider = "System.Data.SqlClient";
                FMDB.ConnectionString = "Data Source=(LocalDB)\\v11.0;AttachDbFilename=\"|DataDirectory|\\FuturemudDatabase.mdf\";Integrated Security=True;MultipleActiveResultSets=True;App=EntityFramework";
                // END TODO

                var fm = new Futuremud(null);
                (fm as IFuturemudLoader).LoadFromDatabase();
                Gameworld = fm;
                
                Console.WriteLine("Initialising Database Context...");
                DB = new FMDB();
                Context = FMDB.Context;
                Console.WriteLine("Done Initialising.");
            });
        }

        #region IDisposable Members

        public void Dispose() {
            if (Gameworld != null) {
                Gameworld.Dispose();
            }

            if (Context != null) {
                Context.Dispose();
            }
        }

        #endregion
    }
}
