-- Create the AcademixDB database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'AcademixDB')
BEGIN
    CREATE DATABASE AcademixDB;
    PRINT 'Database AcademixDB created successfully.';
END
ELSE
BEGIN
    PRINT 'Database AcademixDB already exists.';
END

-- Use the AcademixDB database
USE AcademixDB;


