use Store
go
delete from Departments where Name = 'Test' or Name = 'Test2'
insert into Departments(Name) values ('Test')
select * from Departments

----------------------------- dirty reads -----------------------------

--- Transaction 1 ---
begin transaction
update Departments set Name = 'Test2' where Name = 'Test'
waitfor delay '00:00:05'
rollback transaction



--- Transaction 2 ---
--set transaction isolation level read uncommitted;
set transaction isolation level read committed; -- solution
begin transaction
select * from Departments
waitfor delay '00:00:05'
select * from Departments
commit transaction

------------------------- non-repeatable reads ------------------------

--- Transaction 1 ---
insert into Departments(Name) values ('Test')
begin transaction
waitfor delay '00:00:05'
update Departments set Name = 'Test2' where name = 'Test'
commit transaction

--- Transaction 2 ---
--set transaction isolation level read committed;
set transaction isolation level repeatable read; --solution
begin transaction
select * from Departments
waitfor delay '00:00:05'
select * from Departments
commit transaction

---------------------------- phantom reads ----------------------------

delete from Departments where Name = 'Test' or Name = 'Test2'

--- Transaction 1 ---
begin transaction
waitfor delay '00:00:05'
insert into Departments(Name) values ('Test')
commit transaction

--- Transaction 2 ---
--set transaction isolation level repeatable read
set transaction isolation level serializable -- solution
begin transaction
select * from Departments
waitfor delay '00:00:05'
select * from Departments
commit transaction

------------------------------ deadlock -------------------------------
delete from Departments where Name = 'Test' or Name = 'Test2'
insert into Departments(Name) values ('Test')
delete from ProductTypes where Name = 'Test' or Name = 'Test2'
insert into ProductTypes(Name) values ('Test')

--- Transaction 1 ---
begin transaction
update Departments set Name = 'Test2' where Name = 'Test'
waitfor delay '00:00:05'
update ProductTypes set Name = 'Test2' where Name = 'Test'
commit transaction

--- Transaction 2 ---
set deadlock_priority high -- or normal/low -> change who is chosen as the victim--solution
begin transaction
update ProductTypes set Name = 'Test2' where Name = 'Test'
waitfor delay '00:00:05'
update Departments set Name = 'Test2' where Name = 'Test'
commit transaction

--------------------------- update conflict ---------------------------
--- setup ---
alter database Store set allow_snapshot_isolation on
insert into Departments(Name) values ('Test')
--- Transaction 1 ---
set transaction isolation level snapshot;
begin transaction
update Departments set Name = 'Test2' where Name = 'Test'
waitfor delay '00:00:05'
commit transaction

--- Transaction 2 ---
set transaction isolation level snapshot;
begin transaction
update Departments set Name = 'Test2' where Name = 'Test'
commit transaction

select * from Departments
--- cleanup ---
alter database Store set allow_snapshot_isolation off










create table Sections(
ID INT PRIMARY KEY IDENTITY,
Name varchar(30) not null,
DID int foreign key references Departments(ID) on delete cascade)
select * from Sections
select * from Departments
insert into Sections(name,DID) values ('PhantomReads',1003)

create table ProductTypes
(
ID int primary key identity,
Name varchar(50) not null,
DID int foreign key references Departments(ID) on delete cascade,
)