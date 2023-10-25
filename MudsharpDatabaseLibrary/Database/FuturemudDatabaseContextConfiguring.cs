using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

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
            
            OnModelCreatingPartial(modelBuilder);
        }
    }
}
