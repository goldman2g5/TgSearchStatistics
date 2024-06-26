using Microsoft.EntityFrameworkCore;

namespace TgSearchStatistics.Models.TempScaffold;

public partial class TgDbContext : DbContext
{
    public TgDbContext()
    {
    }

    public TgDbContext(DbContextOptions<TgDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Apikey> Apikeys { get; set; }

    public virtual DbSet<Channel> Channels { get; set; }

    public virtual DbSet<ChannelAccess> ChannelAccesses { get; set; }

    public virtual DbSet<ChannelHasSubscription> ChannelHasSubscriptions { get; set; }

    public virtual DbSet<ChannelHasTag> ChannelHasTags { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<JobScheduleRecord> JobScheduleRecords { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<MonthViewsRecord> MonthViewsRecords { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationDelayedTask> NotificationDelayedTasks { get; set; }

    public virtual DbSet<NotificationSetting> NotificationSettings { get; set; }

    public virtual DbSet<NotificationType> NotificationTypes { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<ReportType> ReportTypes { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<StatisticsSheet> StatisticsSheets { get; set; }

    public virtual DbSet<SubType> SubTypes { get; set; }

    public virtual DbSet<SubscribersRecord> SubscribersRecords { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<TelegramPayment> TelegramPayments { get; set; }

    public virtual DbSet<TgClient> TgClients { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<ViewsRecord> ViewsRecords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=TgDb;Username=postgres;Password=vagina21519687");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Admin_pkey");

            entity.ToTable("Admin");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Key).HasColumnName("key");
            entity.Property(e => e.TelegramId).HasColumnName("telegram_id");
        });

        modelBuilder.Entity<Apikey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("APIkeys_pkey");

            entity.ToTable("APIkeys");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClientName).HasColumnName("client_name");
            entity.Property(e => e.Key).HasColumnName("key");
        });

        modelBuilder.Entity<Channel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Channel_pkey");

            entity.ToTable("Channel");

            entity.HasIndex(e => e.User, "IX_Channel_user");

            entity.HasIndex(e => e.TelegramId, "channel_telegramid_uq").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Avatar).HasColumnName("avatar");
            entity.Property(e => e.Bumps).HasColumnName("bumps");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Flag).HasColumnName("flag");
            entity.Property(e => e.Language).HasColumnName("language");
            entity.Property(e => e.LastBump)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_bump");
            entity.Property(e => e.Members).HasColumnName("members");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.NotificationSent).HasColumnName("notification_sent");
            entity.Property(e => e.Notifications).HasColumnName("notifications");
            entity.Property(e => e.PromoPost).HasColumnName("promo_post");
            entity.Property(e => e.PromoPostInterval).HasColumnName("promo_post_interval");
            entity.Property(e => e.PromoPostLast)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("promo_post_last");
            entity.Property(e => e.PromoPostSent).HasColumnName("promo_post_sent");
            entity.Property(e => e.PromoPostTime).HasColumnName("promo_post_time");
            entity.Property(e => e.TelegramId).HasColumnName("telegram_id");
            entity.Property(e => e.TgclientId).HasColumnName("tgclient_id");
            entity.Property(e => e.User).HasColumnName("user");

            entity.HasOne(d => d.Tgclient).WithMany(p => p.Channels)
                .HasForeignKey(d => d.TgclientId)
                .HasConstraintName("Channel_tgclient_id_fkey");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.Channels)
                .HasForeignKey(d => d.User)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_user_id");
        });

        modelBuilder.Entity<ChannelAccess>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ChannelAccess_pkey");

            entity.ToTable("ChannelAccess");

            entity.HasIndex(e => e.ChannelId, "IX_ChannelAccess_channel_id");

            entity.HasIndex(e => e.UserId, "IX_ChannelAccess_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChannelId).HasColumnName("channel_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Channel).WithMany(p => p.ChannelAccesses)
                .HasForeignKey(d => d.ChannelId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_channelaccess_channel");

            entity.HasOne(d => d.User).WithMany(p => p.ChannelAccesses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("user_fk");
        });

        modelBuilder.Entity<ChannelHasSubscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ChannelHasSubscription_pkey");

            entity.ToTable("ChannelHasSubscription");

            entity.HasIndex(e => e.ChannelId, "IX_ChannelHasSubscription_channel_id");

            entity.HasIndex(e => e.TypeId, "IX_ChannelHasSubscription_type_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChannelId).HasColumnName("channel_id");
            entity.Property(e => e.Expires)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expires");
            entity.Property(e => e.TypeId).HasColumnName("type_id");

            entity.HasOne(d => d.Channel).WithMany(p => p.ChannelHasSubscriptions)
                .HasForeignKey(d => d.ChannelId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_channelhassubscription_channel");

            entity.HasOne(d => d.Type).WithMany(p => p.ChannelHasSubscriptions)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("fk_Sub_type");
        });

        modelBuilder.Entity<ChannelHasTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ChannelHasTag_pkey");

            entity.ToTable("ChannelHasTag");

            entity.HasIndex(e => e.Channel, "IX_ChannelHasTag_channel");

            entity.HasIndex(e => e.Tag, "IX_ChannelHasTag_tag");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Channel).HasColumnName("channel");
            entity.Property(e => e.Tag).HasColumnName("tag");

            entity.HasOne(d => d.ChannelNavigation).WithMany(p => p.ChannelHasTags)
                .HasForeignKey(d => d.Channel)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_channelhastag_channel");

            entity.HasOne(d => d.TagNavigation).WithMany(p => p.ChannelHasTags)
                .HasForeignKey(d => d.Tag)
                .HasConstraintName("fk_ChannelHasTag_Tag");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Comment_pkey");

            entity.ToTable("Comment");

            entity.HasIndex(e => e.ChannelId, "IX_Comment_channel_id");

            entity.HasIndex(e => e.ParentId, "IX_Comment_parent_id");

            entity.HasIndex(e => e.UserId, "IX_Comment_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChannelId).HasColumnName("channel_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Channel).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ChannelId)
                .HasConstraintName("fk_comment_channel");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("parent_id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_id_fk");
        });

        modelBuilder.Entity<JobScheduleRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("JobSchedules_pkey");

            entity.ToTable("JobScheduleRecord");

            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Message_pkey");

            entity.ToTable("Message");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ChannelTelegramId).HasColumnName("channel_telegram_id");
            entity.Property(e => e.Media).HasColumnName("media");
            entity.Property(e => e.Text).HasColumnName("text");
            entity.Property(e => e.Views).HasColumnName("views");

            entity.HasOne(d => d.ChannelTelegram).WithMany(p => p.Messages)
                .HasPrincipalKey(p => p.TelegramId)
                .HasForeignKey(d => d.ChannelTelegramId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("channel_telegram_id_fk");
        });

        modelBuilder.Entity<MonthViewsRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("MonthViewsRecord_pkey");

            entity.ToTable("MonthViewsRecord");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.LastMessageId).HasColumnName("last_message_id");
            entity.Property(e => e.Sheet).HasColumnName("sheet");
            entity.Property(e => e.Updated).HasColumnName("updated");
            entity.Property(e => e.Views).HasColumnName("views");

            entity.HasOne(d => d.SheetNavigation).WithMany(p => p.MonthViewsRecords)
                .HasForeignKey(d => d.Sheet)
                .HasConstraintName("sheet_fk");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Notification_pkey");

            entity.ToTable("Notification");

            entity.HasIndex(e => e.ChannelId, "IX_Notification_channel_id");

            entity.HasIndex(e => e.TypeId, "IX_Notification_type_id");

            entity.HasIndex(e => e.UserId, "IX_Notification_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChannelId).HasColumnName("channel_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.ContentType).HasColumnName("content_type");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.IsNew).HasColumnName("is_new");
            entity.Property(e => e.TargetTelegram).HasColumnName("target_telegram");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Channel).WithMany(p => p.NotificationsNavigation)
                .HasForeignKey(d => d.ChannelId)
                .HasConstraintName("channel_id_fk");

            entity.HasOne(d => d.Type).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("type_id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_id_fk");
        });

        modelBuilder.Entity<NotificationDelayedTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("NotificationDelayedTask_pkey");

            entity.ToTable("NotificationDelayedTask");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChannelId).HasColumnName("channel_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.ContentType).HasColumnName("content_type");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.TargetTelegram).HasColumnName("target_telegram");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Channel).WithMany(p => p.NotificationDelayedTasks)
                .HasForeignKey(d => d.ChannelId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("channel_id_fk");

            entity.HasOne(d => d.Type).WithMany(p => p.NotificationDelayedTasks)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("type_id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.NotificationDelayedTasks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_id_fk");
        });

        modelBuilder.Entity<NotificationSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("NotificationSettings_pkey");

            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<NotificationType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("NotificationType_pkey");

            entity.ToTable("NotificationType");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Text).HasColumnName("text");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payments_pkey1");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AmountCurrency)
                .HasMaxLength(3)
                .HasColumnName("amount_currency");
            entity.Property(e => e.AmountValue).HasColumnName("amount_value");
            entity.Property(e => e.Capture).HasColumnName("capture");
            entity.Property(e => e.CaptureJson)
                .HasColumnType("json")
                .HasColumnName("capture_json");
            entity.Property(e => e.CapturedAt).HasColumnName("captured_at");
            entity.Property(e => e.ChannelId).HasColumnName("channel_id");
            entity.Property(e => e.ClientIp)
                .HasMaxLength(255)
                .HasColumnName("client_ip");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.FullJson)
                .HasColumnType("json")
                .HasColumnName("full_json");
            entity.Property(e => e.Paid).HasColumnName("paid");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(255)
                .HasColumnName("payment_method");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.SubGiven).HasColumnName("sub_given");
            entity.Property(e => e.SubtypeId).HasColumnName("subtype_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Channel).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ChannelId)
                .HasConstraintName("channel_fk");

            entity.HasOne(d => d.Subtype).WithMany(p => p.Payments)
                .HasForeignKey(d => d.SubtypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("subtype_fk");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_fk");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Report_pkey");

            entity.ToTable("Report");

            entity.HasIndex(e => e.ChannelId, "IX_Report_channel_id");

            entity.HasIndex(e => e.CommentId, "IX_Report_comment_id");

            entity.HasIndex(e => e.ReportType, "IX_Report_report_type");

            entity.HasIndex(e => e.StaffId, "IX_Report_staff_id");

            entity.HasIndex(e => e.UserId, "IX_Report_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChannelId).HasColumnName("channel_id");
            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.NotificationSent).HasColumnName("notification_sent");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.ReportTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("report_time");
            entity.Property(e => e.ReportType).HasColumnName("report_type");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Text).HasColumnName("text");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Channel).WithMany(p => p.Reports)
                .HasForeignKey(d => d.ChannelId)
                .HasConstraintName("channel_id_fk");

            entity.HasOne(d => d.Comment).WithMany(p => p.Reports)
                .HasForeignKey(d => d.CommentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("comment_id_fk");

            entity.HasOne(d => d.ReportTypeNavigation).WithMany(p => p.Reports)
                .HasForeignKey(d => d.ReportType)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("report_type_fk");

            entity.HasOne(d => d.Staff).WithMany(p => p.Reports)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("staff_id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.Reports)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id_fk");
        });

        modelBuilder.Entity<ReportType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ReportType_pkey");

            entity.ToTable("ReportType");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Text).HasColumnName("text");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Staff_pkey");

            entity.HasIndex(e => e.UserId, "IX_Staff_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Staff)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id_fk");
        });

        modelBuilder.Entity<StatisticsSheet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("StatisticsSheet_pkey");

            entity.ToTable("StatisticsSheet");

            entity.HasIndex(e => e.ChannelId, "IX_StatisticsSheet_channel_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChannelId).HasColumnName("channel_id");

            entity.HasOne(d => d.Channel).WithMany(p => p.StatisticsSheets)
                .HasForeignKey(d => d.ChannelId)
                .HasConstraintName("channel_id_fk");
        });

        modelBuilder.Entity<SubType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SubType_pkey");

            entity.ToTable("SubType");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Multiplier).HasColumnName("multiplier");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.TagLimit).HasColumnName("tag_limit");
        });

        modelBuilder.Entity<SubscribersRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SubscribersRecord_pkey");

            entity.ToTable("SubscribersRecord");

            entity.HasIndex(e => e.Sheet, "IX_SubscribersRecord_sheet");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Sheet).HasColumnName("sheet");
            entity.Property(e => e.Subscribers).HasColumnName("subscribers");

            entity.HasOne(d => d.SheetNavigation).WithMany(p => p.SubscribersRecords)
                .HasForeignKey(d => d.Sheet)
                .HasConstraintName("sub_sheet_fk");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Tag_pkey");

            entity.ToTable("Tag");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Text).HasColumnName("text");
        });

        modelBuilder.Entity<TelegramPayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payments_pkey");

            entity.HasIndex(e => e.ChannelId, "IX_payments_channelId");

            entity.HasIndex(e => e.SubscriptionTypeId, "IX_payments_subscription_type_id");

            entity.HasIndex(e => e.UserId, "IX_payments_userId");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.AutoRenewal).HasColumnName("autoRenewal");
            entity.Property(e => e.ChannelId).HasColumnName("channelId");
            entity.Property(e => e.ChannelName).HasColumnName("channel_name");
            entity.Property(e => e.Discount).HasColumnName("discount");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.Expires)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expires");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.SubscriptionTypeId).HasColumnName("subscription_type_id");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.Username).HasColumnName("username");

            entity.HasOne(d => d.Channel).WithMany(p => p.TelegramPayments)
                .HasForeignKey(d => d.ChannelId)
                .HasConstraintName("channel_id_fk");

            entity.HasOne(d => d.SubscriptionType).WithMany(p => p.TelegramPayments)
                .HasForeignKey(d => d.SubscriptionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("subtype_fk");

            entity.HasOne(d => d.User).WithMany(p => p.TelegramPayments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id_fk");
        });

        modelBuilder.Entity<TgClient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TgClient_pkey");

            entity.ToTable("TgClient");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.ApiHash).HasColumnName("api_hash");
            entity.Property(e => e.ApiId).HasColumnName("api_id");
            entity.Property(e => e.ChannelCount).HasColumnName("channel_count");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
            entity.Property(e => e.TelegramId).HasColumnName("telegram_id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pkey");

            entity.ToTable("User");

            entity.HasIndex(e => e.NotificationSettings, "IX_User_notification_settings");

            entity.HasIndex(e => e.ChatId, "chat_id_uq").IsUnique();

            entity.HasIndex(e => e.TelegramId, "telegram_id_uq").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Avatar).HasColumnName("avatar");
            entity.Property(e => e.ChatId).HasColumnName("chat_id");
            entity.Property(e => e.LastUpdate).HasColumnName("last_update");
            entity.Property(e => e.NotificationSettings).HasColumnName("notification_settings");
            entity.Property(e => e.TelegramId).HasColumnName("telegram_id");
            entity.Property(e => e.UniqueKey).HasColumnName("unique_key");
            entity.Property(e => e.Username).HasColumnName("username");

            entity.HasOne(d => d.NotificationSettingsNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.NotificationSettings)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("notification_settings_fk");
        });

        modelBuilder.Entity<ViewsRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ViewsRecord_pkey");

            entity.ToTable("ViewsRecord");

            entity.HasIndex(e => e.Sheet, "IX_ViewsRecord_sheet");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.LastMessageId).HasColumnName("last_message_id");
            entity.Property(e => e.Sheet).HasColumnName("sheet");
            entity.Property(e => e.Updated).HasColumnName("updated");
            entity.Property(e => e.Views).HasColumnName("views");

            entity.HasOne(d => d.SheetNavigation).WithMany(p => p.ViewsRecords)
                .HasForeignKey(d => d.Sheet)
                .HasConstraintName("StatisticsSheet_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
