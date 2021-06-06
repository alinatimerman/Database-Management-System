use Store
go

create table Departments(
ID int primary key identity,
Name varchar(50) not null)

create table Employees(
CNP varchar(13)primary key,
FirstName varchar(50) not null,
LastName varchar(50) not null,
Email varchar(50),
Phone varchar(10),
Salary decimal(7,2) not null,
Age int)

create table Departments_Employees(
DID int foreign key references Departments(ID) on delete cascade,
CNP varchar(13) foreign key references Employees(CNP) on delete cascade,
constraint PK_Departments_Employees
primary key(DID,CNP)
)




select * from Employees
select * from Departments
select * from Departments_Employees

-- Create a stored procedure that inserts data in tables that are in a m:n relationship. If one insert
-- fails, all the operations performed by the procedure must be rolled back. 



create function uf_ValidateDepartment(@name varchar(50)) returns bit as
begin
    declare @return int
	set @return=0
    if (@name like '[A-Z]%')
        set @return = 1
    return @return;
end

create function uf_ValidateCNP(@cnp varchar(13)) returns bit as
begin
    declare @return int
	set @return=0
    if (len(@cnp)<13)
        set @return = 1
    return @return;
end


create or alter procedure AddDepartmentEmployee @d_name varchar(50),
												@e_cnp varchar(13),
												@e_f_name varchar(50),
												@e_l_name varchar(50),
												@e_email varchar(50),
												@e_phone varchar(10),
												@e_salary decimal(7, 2),
												@e_age int as
begin
    declare @did int;
    begin transaction
        begin try
            if (dbo.uf_ValidateDepartment(@d_name) <> 1)
                raiserror ('Invalid department name', 14, 1);
            insert into Departments (Name) values (@d_name);
            set @did = @@IDENTITY;
            print concat('Inserted Department with name ' + @d_name + ', id ', cast(@did as varchar(10)));
            insert into Employees(CNP, FirstName, LastName, Email, Phone, Salary, Age)
                values (@e_cnp, @e_f_name, @e_l_name, @e_email, @e_phone, @e_salary, @e_age);
            print concat('Inserted Employee with cnp ', cast(@e_cnp as varchar(10)));
            insert into Departments_Employees(did, cnp) values (@did, @e_cnp);
            print concat('Connected department with id ', cast(@did as varchar(10)), ' with employee with cnp ', @e_cnp);
            commit transaction;
            print('Transaction committed');
        end try
        begin catch
            rollback transaction;
            print concat('Transaction rolled back; cause: ', ERROR_MESSAGE());
        end catch
end

exec dbo.AddDepartmentEmployee
    'Department1',   --ok
    '3241785679390',
    'Adriana',
    'Ionescu',
    'abcsss@gmail.com',
    '0795654509',
     5300.00,
     22;

exec dbo.AddDepartmentEmployee
    '12Dep1', -- validation error
    '6219995017811',
    'Ana',
    'Popescu',
    'anaana@gmail.com',
    '0723457362',
     4200.00,
     23;

exec dbo.AddDepartmentEmployee --primary key constraint error
    'Test_Dep',
    '6210505018433',
    'FName',
    'LName',
    'f@l.com',
    '0723457362',
    4200.00,
    33;

select D.ID as DID,
       E.CNP as ECNP,
       D.Name as DName,
       E.FirstName as EFName,
       E.LastName as ELName,
       E.Email as EEmail,
       E.Phone as EPhone,
       E.Salary as ESalary,
       E.Age as EAge
       from Employees E
    inner join Departments_Employees DE on E.CNP = DE.CNP
    inner join Departments D on D.ID = DE.DID