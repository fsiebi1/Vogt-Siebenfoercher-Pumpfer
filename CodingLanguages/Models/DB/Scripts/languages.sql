drop database languages;
create database languages collate utf8mb4_general_ci;
use languages;

create table User(
	user_id int unsigned not null auto_increment,
	username varchar(100) not null unique,
	firstname varchar(100) not null,
	lastname varchar(100) not null,
	password varchar(300) not null,
	birthdate date not null,
	email varchar(100) not null unique,
	country varchar(100) not null,
	gender int not null,
	admin int not null,

	constraint user_id_PK primary key(user_id)
);

insert into user values(null, "hallo", "franz", "fritz", sha2("12345678", 512), "2004-05-03", "hallo@raeucher.info", "Austria", 1, 1);

-- email bestaetigen + newsletter einfuegen
