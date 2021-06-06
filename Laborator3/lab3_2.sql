-- create a stored procedure that inserts data in tables that are in a m:n relationship; if an insert fails,
-- try to recover as much as possible from the entire operation: for example, if the user wants to add
-- an employee and its department, succeeds creating the employee, but fails with the department, the employee
-- remains in the database;

use Store
go

create or alter procedure AddDepartmentEmployeeMultipleTransactions @d_name varchar(50),
                                                          @e_cnp varchar(13),
                                                          @e_f_name varchar(50),
                                                          @e_l_name varchar(50),
                                                          @e_email varchar(50),
                                                          @e_phone varchar(10),
                                                          @e_salary decimal(7, 2),
                                                          @e_age int as
begin
    declare @did int;
    declare @error bit = 0;
    begin transaction;
        begin try
            print concat('Trying to insert Employee with cnp ', cast(@e_cnp as varchar(10)));
            insert into Employees(CNP, FirstName, LastName, Email, Phone, Salary, Age)
            values (@e_cnp, @e_f_name, @e_l_name, @e_email, @e_phone, @e_salary, @e_age);
            print concat('Inserted Employee with cnp ', cast(@e_cnp as varchar(10)));
            commit transaction;
        end try
        begin catch
            rollback transaction;
            print concat('Transaction rolled back; cause: ', ERROR_MESSAGE());
            set @error = 1;
        end catch;
    begin transaction;
        begin try
            print concat('Trying to insert Department with name ', @d_name);
            if (dbo.uf_ValidateDepartment(@d_name) <> 1)
                raiserror ('Invalid department name', 14, 1);
            insert into Departments (Name) values (@d_name);
            set @did = @@IDENTITY;
            print concat('Inserted Department with name ', @d_name, ', id ', cast(@did as varchar(10)));
            commit transaction;
        end try
        begin catch
            rollback transaction;
            print concat('Transaction rolled back; cause: ', ERROR_MESSAGE());
            set @error = 1;
        end catch;
    if (@error = 1)
        return;
    begin transaction;
        begin try
            insert into Departments_Employees(did, cnp) values (@did, @e_cnp);
            print concat('Connected department with id ', cast(@did as varchar(10)), ' with employee with cnp ',
                         @e_cnp);
            commit transaction;
        end try
        begin catch
            rollback transaction;
            print concat('Transaction rolled back; cause: ', ERROR_MESSAGE());
        end catch
end;

--delete from Departments_Employees
--where DID = 4 and CNP = '6210505018411';
--delete from Employees where CNP = '6210505018411';
--delete from Departments where Name = 'Test';

exec dbo.AddDepartmentEmployeeMultipleTransactions
    '1zzz', -- validation error -> employee still added
    '9210505018499',
    'ABCDE',
    'Abcdefgh',
    'abcdef@gmail.com',
    '0723457362',
    4600.00,
    22;

exec dbo.AddDepartmentEmployeeMultipleTransactions -- twice for key constraint error
    'Test_dep2',
    '9210505018420',
    'Ghifg',
    'Ltesttest',
    'aaa@lmail.com',
    '0723457362',
    4200.00,
    34;

select * from Employees 
select * from Departments;

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