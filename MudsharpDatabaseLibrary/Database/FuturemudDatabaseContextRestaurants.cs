using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

#nullable enable

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	private static void ConfigureRestaurants(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Restaurant>(entity =>
		{
			entity.ToTable("Restaurants");
			entity.HasKey(e => e.ShopId).HasName("PRIMARY");
			entity.Property(e => e.ShopId).HasColumnType("bigint(20)");
			entity.Property(e => e.AutomatedService).HasColumnType("bit(1)").HasDefaultValue(false);
			entity.Property(e => e.SimulateCrafting).HasColumnType("bit(1)").HasDefaultValue(false);
			entity.Property(e => e.HandlingSeconds).HasColumnType("int(11)").HasDefaultValue(15);
			entity.Property(e => e.MaximumBatchWaitSeconds).HasColumnType("int(11)").HasDefaultValue(90);
			entity.Property(e => e.CleanupIntervalSeconds).HasColumnType("int(11)").HasDefaultValue(120);
			entity.Property(e => e.ChefStartEmote).HasColumnType("varchar(1000)").HasDefaultValue("@ begin|begins preparing $0.");
			entity.Property(e => e.ChefOpenEmote).HasColumnType("varchar(1000)").HasDefaultValue("@ open|opens $0 for service.");
			entity.Property(e => e.ChefPlateEmote).HasColumnType("varchar(1000)").HasDefaultValue("@ plate|plates $0 on $1.");
			entity.Property(e => e.ChefReadyEmote).HasColumnType("varchar(1000)").HasDefaultValue("@ finish|finishes preparing $0 for service.");
			entity.Property(e => e.ServerServeEmote).HasColumnType("varchar(1000)").HasDefaultValue("@ place|places $0 before $1 on $2.");
			entity.Property(e => e.ServerClearEmote).HasColumnType("varchar(1000)").HasDefaultValue("@ clear|clears $0 from $1.");
			entity.Property(e => e.ServerReturnEmote).HasColumnType("varchar(1000)").HasDefaultValue("@ put|puts $0 aside in the kitchen.");
			entity.HasOne(e => e.Shop)
			      .WithOne(e => e.Restaurant)
			      .HasForeignKey<Restaurant>(e => e.ShopId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_Restaurants_Shops");
		});

		modelBuilder.Entity<RestaurantCell>(entity =>
		{
			entity.ToTable("RestaurantCells");
			entity.HasKey(e => new { e.RestaurantShopId, e.CellId, e.Role }).HasName("PRIMARY");
			entity.HasIndex(e => e.CellId).HasDatabaseName("IX_RestaurantCells_Cell");
			entity.HasIndex(e => new { e.RestaurantShopId, e.Role }).HasDatabaseName("IX_RestaurantCells_Restaurant_Role");
			entity.Property(e => e.RestaurantShopId).HasColumnType("bigint(20)");
			entity.Property(e => e.CellId).HasColumnType("bigint(20)");
			entity.Property(e => e.Role).HasColumnType("int(11)");
			entity.HasOne(e => e.Restaurant)
			      .WithMany(e => e.Cells)
			      .HasForeignKey(e => e.RestaurantShopId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_RestaurantCells_Restaurants");
		});

		modelBuilder.Entity<RestaurantTable>(entity =>
		{
			entity.ToTable("RestaurantTables");
			entity.HasKey(e => new { e.RestaurantShopId, e.GameItemId }).HasName("PRIMARY");
			entity.HasIndex(e => e.GameItemId).HasDatabaseName("IX_RestaurantTables_GameItem");
			entity.Property(e => e.RestaurantShopId).HasColumnType("bigint(20)");
			entity.Property(e => e.GameItemId).HasColumnType("bigint(20)");
			entity.HasOne(e => e.Restaurant)
			      .WithMany(e => e.Tables)
			      .HasForeignKey(e => e.RestaurantShopId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_RestaurantTables_Restaurants");
		});

		modelBuilder.Entity<RestaurantMenuItem>(entity =>
		{
			entity.ToTable("RestaurantMenuItems");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.RestaurantShopId).HasDatabaseName("FK_RestaurantMenuItems_Restaurants_idx");
			entity.HasIndex(e => e.MerchandiseId).HasDatabaseName("FK_RestaurantMenuItems_Merchandises_idx");
			entity.HasIndex(e => new { e.RestaurantShopId, e.SortOrder }).HasDatabaseName("IX_RestaurantMenuItems_Restaurant_Sort");
			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.RestaurantShopId).HasColumnType("bigint(20)");
			entity.Property(e => e.MerchandiseId).HasColumnType("bigint(20)");
			entity.Property(e => e.Description).RequiredString("mediumtext");
			entity.Property(e => e.FulfilmentMode).HasColumnType("int(11)");
			entity.Property(e => e.IsActive).HasColumnType("bit(1)").HasDefaultValue(true);
			entity.Property(e => e.DineInAvailable).HasColumnType("bit(1)").HasDefaultValue(true);
			entity.Property(e => e.TakeawayAvailable).HasColumnType("bit(1)").HasDefaultValue(true);
			entity.Property(e => e.PreparationSeconds).HasColumnType("int(11)");
			entity.Property(e => e.CraftId).HasColumnType("bigint(20)");
			entity.Property(e => e.CraftRevisionNumber).HasColumnType("int(11)");
			entity.Property(e => e.ServingContainerPrototypeId).HasColumnType("bigint(20)");
			entity.Property(e => e.ServingContainerPrototypeRevisionNumber).HasColumnType("int(11)");
			entity.Property(e => e.TakeawayContainerPrototypeId).HasColumnType("bigint(20)");
			entity.Property(e => e.TakeawayContainerPrototypeRevisionNumber).HasColumnType("int(11)");
			entity.Property(e => e.TakeawayBagPrototypeId).HasColumnType("bigint(20)");
			entity.Property(e => e.TakeawayBagPrototypeRevisionNumber).HasColumnType("int(11)");
			entity.Property(e => e.SortOrder).HasColumnType("int(11)");
			entity.HasOne(e => e.Restaurant)
			      .WithMany(e => e.MenuItems)
			      .HasForeignKey(e => e.RestaurantShopId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_RestaurantMenuItems_Restaurants");
			entity.HasOne(e => e.Merchandise)
			      .WithMany()
			      .HasForeignKey(e => e.MerchandiseId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_RestaurantMenuItems_Merchandises");
		});

		modelBuilder.Entity<RestaurantTableSession>(entity =>
		{
			entity.ToTable("RestaurantTableSessions");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.RestaurantShopId).HasDatabaseName("FK_RestaurantTableSessions_Restaurants_idx");
			entity.HasIndex(e => new { e.RestaurantShopId, e.TableGameItemId, e.Status }).HasDatabaseName("IX_RestaurantTableSessions_Table_Status");
			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.RestaurantShopId).HasColumnType("bigint(20)");
			entity.Property(e => e.TableGameItemId).HasColumnType("bigint(20)");
			entity.Property(e => e.Status).HasColumnType("int(11)");
			entity.Property(e => e.CreatedAtUtc).HasColumnType("datetime(6)");
			entity.Property(e => e.LastUpdatedAtUtc).HasColumnType("datetime(6)");
			entity.Property(e => e.ClosedAtUtc).HasColumnType("datetime(6)");
			entity.Property(e => e.AbandonmentPendingAtUtc).HasColumnType("datetime(6)");
			entity.Property(e => e.AbandonmentReported).HasColumnType("bit(1)").HasDefaultValue(false);
			entity.HasOne(e => e.Restaurant)
			      .WithMany(e => e.TableSessions)
			      .HasForeignKey(e => e.RestaurantShopId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_RestaurantTableSessions_Restaurants");
		});

		modelBuilder.Entity<RestaurantTableParticipant>(entity =>
		{
			entity.ToTable("RestaurantTableParticipants");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.RestaurantTableSessionId).HasDatabaseName("FK_RestaurantTableParticipants_Sessions_idx");
			entity.HasIndex(e => new { e.RestaurantTableSessionId, e.CharacterId }).HasDatabaseName("IX_RestaurantTableParticipants_Session_Character").IsUnique();
			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.RestaurantTableSessionId).HasColumnType("bigint(20)");
			entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");
			entity.Property(e => e.CharacterName).RequiredString("mediumtext");
			entity.Property(e => e.Accepted).HasColumnType("bit(1)");
			entity.Property(e => e.JoinedAtUtc).HasColumnType("datetime(6)");
			entity.Property(e => e.LeftAtUtc).HasColumnType("datetime(6)");
			entity.HasOne(e => e.Session)
			      .WithMany(e => e.Participants)
			      .HasForeignKey(e => e.RestaurantTableSessionId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_RestaurantTableParticipants_Sessions");
		});

		modelBuilder.Entity<RestaurantOrder>(entity =>
		{
			entity.ToTable("RestaurantOrders");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.RestaurantShopId).HasDatabaseName("FK_RestaurantOrders_Restaurants_idx");
			entity.HasIndex(e => e.RestaurantTableSessionId).HasDatabaseName("FK_RestaurantOrders_Sessions_idx");
			entity.HasIndex(e => e.RestaurantMenuItemId).HasDatabaseName("FK_RestaurantOrders_MenuItems_idx");
			entity.HasIndex(e => new { e.RestaurantShopId, e.Status }).HasDatabaseName("IX_RestaurantOrders_Restaurant_Status");
			entity.HasIndex(e => new { e.RestaurantTableSessionId, e.OrdererCharacterId }).HasDatabaseName("IX_RestaurantOrders_Session_Debtor");
			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.RestaurantShopId).HasColumnType("bigint(20)");
			entity.Property(e => e.RestaurantTableSessionId).HasColumnType("bigint(20)");
			entity.Property(e => e.RestaurantMenuItemId).HasColumnType("bigint(20)");
			entity.Property(e => e.OrderType).HasColumnType("int(11)");
			entity.Property(e => e.Status).HasColumnType("int(11)");
			entity.Property(e => e.OrdererCharacterId).HasColumnType("bigint(20)");
			entity.Property(e => e.OrdererCharacterName).RequiredString("mediumtext");
			entity.Property(e => e.RecipientCharacterId).HasColumnType("bigint(20)");
			entity.Property(e => e.RecipientCharacterName).RequiredString("mediumtext");
			entity.Property(e => e.Quantity).HasColumnType("int(11)");
			entity.Property(e => e.PretaxPrice).HasColumnType("decimal(58,29)");
			entity.Property(e => e.Tax).HasColumnType("decimal(58,29)");
			entity.Property(e => e.Price).HasColumnType("decimal(58,29)");
			entity.Property(e => e.AmountPaid).HasColumnType("decimal(58,29)");
			entity.Property(e => e.RevenueRecognised).HasColumnType("bit(1)").HasDefaultValue(false);
			entity.Property(e => e.CreatedAtUtc).HasColumnType("datetime(6)");
			entity.Property(e => e.LastUpdatedAtUtc).HasColumnType("datetime(6)");
			entity.Property(e => e.ExpectedReadyAtUtc).HasColumnType("datetime(6)");
			entity.Property(e => e.ReadyAtUtc).HasColumnType("datetime(6)");
			entity.Property(e => e.ServedAtUtc).HasColumnType("datetime(6)");
			entity.Property(e => e.PreparedByEmployeeId).HasColumnType("bigint(20)");
			entity.Property(e => e.ServedByEmployeeId).HasColumnType("bigint(20)");
			entity.Property(e => e.OperationalNotes).RequiredString("mediumtext");
			entity.HasOne(e => e.Restaurant)
			      .WithMany(e => e.Orders)
			      .HasForeignKey(e => e.RestaurantShopId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_RestaurantOrders_Restaurants");
			entity.HasOne(e => e.TableSession)
			      .WithMany(e => e.Orders)
			      .HasForeignKey(e => e.RestaurantTableSessionId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_RestaurantOrders_Sessions");
			entity.HasOne(e => e.MenuItem)
			      .WithMany()
			      .HasForeignKey(e => e.RestaurantMenuItemId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_RestaurantOrders_MenuItems");
		});

		modelBuilder.Entity<RestaurantPayment>(entity =>
		{
			entity.ToTable("RestaurantPayments");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.RestaurantOrderId).HasDatabaseName("FK_RestaurantPayments_Orders_idx");
			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.RestaurantOrderId).HasColumnType("bigint(20)");
			entity.Property(e => e.PayerCharacterId).HasColumnType("bigint(20)");
			entity.Property(e => e.PayerCharacterName).RequiredString("mediumtext");
			entity.Property(e => e.Amount).HasColumnType("decimal(58,29)");
			entity.Property(e => e.IsRefund).HasColumnType("bit(1)").HasDefaultValue(false);
			entity.Property(e => e.PaymentMethod).RequiredString("varchar(200)");
			entity.Property(e => e.Reference).RequiredString("mediumtext");
			entity.Property(e => e.PaidAtUtc).HasColumnType("datetime(6)");
			entity.HasOne(e => e.Order)
			      .WithMany(e => e.Payments)
			      .HasForeignKey(e => e.RestaurantOrderId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_RestaurantPayments_Orders");
		});

		modelBuilder.Entity<RestaurantOrderItem>(entity =>
		{
			entity.ToTable("RestaurantOrderItems");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.RestaurantOrderId).HasDatabaseName("FK_RestaurantOrderItems_Orders_idx");
			entity.HasIndex(e => e.GameItemId).HasDatabaseName("IX_RestaurantOrderItems_GameItem");
			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.RestaurantOrderId).HasColumnType("bigint(20)");
			entity.Property(e => e.GameItemId).HasColumnType("bigint(20)");
			entity.Property(e => e.Role).HasColumnType("int(11)");
			entity.Property(e => e.Delivered).HasColumnType("bit(1)").HasDefaultValue(false);
			entity.Property(e => e.CreatedAtUtc).HasColumnType("datetime(6)");
			entity.Property(e => e.DeliveredAtUtc).HasColumnType("datetime(6)");
			entity.HasOne(e => e.Order)
			      .WithMany(e => e.ProducedItems)
			      .HasForeignKey(e => e.RestaurantOrderId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_RestaurantOrderItems_Orders");
		});
	}
}
