
      CREATE TABLE [AspNetRoles] (
          [Id] nvarchar(450) NOT NULL,
          [ConcurrencyStamp] nvarchar(max) NULL,
          [Name] nvarchar(256) NULL,
          [NormalizedName] nvarchar(256) NULL,
          CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
      )

      CREATE TABLE [AspNetUserTokens] (
          [UserId] nvarchar(450) NOT NULL,
          [LoginProvider] nvarchar(450) NOT NULL,
          [Name] nvarchar(450) NOT NULL,
          [Value] nvarchar(max) NULL,
          CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name])
      )

      CREATE TABLE [AspNetUsers] (
          [Id] nvarchar(450) NOT NULL,
          [AccessFailedCount] int NOT NULL,
          [ConcurrencyStamp] nvarchar(max) NULL,
          [Email] nvarchar(256) NULL,
          [EmailConfirmed] bit NOT NULL,
          [LockoutEnabled] bit NOT NULL,
          [LockoutEnd] datetimeoffset NULL,
          [NormalizedEmail] nvarchar(256) NULL,
          [NormalizedUserName] nvarchar(256) NULL,
          [PasswordHash] nvarchar(max) NULL,
          [PhoneNumber] nvarchar(max) NULL,
          [PhoneNumberConfirmed] bit NOT NULL,
          [SecurityStamp] nvarchar(max) NULL,
          [TwoFactorEnabled] bit NOT NULL,
          [UserName] nvarchar(256) NULL,
          CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
      )
      CREATE TABLE [AspNetRoleClaims] (
          [Id] int NOT NULL IDENTITY,
          [ClaimType] nvarchar(max) NULL,
          [ClaimValue] nvarchar(max) NULL,
          [RoleId] nvarchar(450) NOT NULL,
          CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
          CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
      )
      CREATE TABLE [AspNetUserClaims] (
          [Id] int NOT NULL IDENTITY,
          [ClaimType] nvarchar(max) NULL,
          [ClaimValue] nvarchar(max) NULL,
          [UserId] nvarchar(450) NOT NULL,
          CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
          CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
      )
      CREATE TABLE [AspNetUserLogins] (
          [LoginProvider] nvarchar(450) NOT NULL,
          [ProviderKey] nvarchar(450) NOT NULL,
          [ProviderDisplayName] nvarchar(max) NULL,
          [UserId] nvarchar(450) NOT NULL,
          CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
          CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
      )
      CREATE TABLE [AspNetUserRoles] (
          [UserId] nvarchar(450) NOT NULL,
          [RoleId] nvarchar(450) NOT NULL,
          CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
          CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
          CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
      )
      CREATE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]);
      CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId])
      CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId])
      CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId])
      CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
      CREATE INDEX [IX_AspNetUserRoles_UserId] ON [AspNetUserRoles] ([UserId])
      CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail])
      CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName])
     
      DROP TABLE [AspNetRoleClaims]
      DROP TABLE [AspNetUserClaims];
      DROP TABLE [AspNetUserLogins];
      DROP TABLE [AspNetUserRoles];

      DROP TABLE [AspNetUserTokens];

      DROP TABLE [AspNetRoles];

      DROP TABLE [AspNetUsers];

      CREATE TABLE [Settings] (
          [ID] int NOT NULL IDENTITY,
          [WaitSecond] nvarchar(max) NULL,
          CONSTRAINT [PK_Settings] PRIMARY KEY ([ID])
      )
      CREATE TABLE [Site] (
          [ID] int NOT NULL IDENTITY,
          [URL] nvarchar(max) NULL,
          CONSTRAINT [PK_Site] PRIMARY KEY ([ID])
      );

      
      ALTER TABLE [Site] ADD [LastCheckedTime] datetime2 NULL;

      ALTER TABLE [Site] ADD [Status] int NULL;
