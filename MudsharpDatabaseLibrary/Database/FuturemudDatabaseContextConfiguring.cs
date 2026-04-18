using Microsoft.EntityFrameworkCore;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.Database
{
    public partial class FuturemudDatabaseContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Split the OnModelCreating into multiple functions for editor performance reasons
            OnModelCreatingOne(modelBuilder);
            OnModelCreatingTwo(modelBuilder);
            OnModelCreatingThree(modelBuilder);
            OnModelCreatingFour(modelBuilder);
            OnModelCreatingFive(modelBuilder);
            OnModelCreatingSix(modelBuilder);

            OnModelCreatingPartial(modelBuilder);
        }
    }
}
