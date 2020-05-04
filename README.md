# File cabinet application
## Command line parameters for FileCabinetApp:
### validation
Example: 
- --validation-rules=Default
- -v:custom
### type of service
Example: 
- --storage=file
- -s:memory
### diagnostics
- --use-logger
- --use-stopwatch
#### FileCabinetMemoryService.
Provides memory service for working with records.
#### FileCabinetFilesystemService.
Provides filesystem service for working with records in binary format.
## Available command:
### create
Creating new record. 
### export
Exporting data of record into specific doc type. 
- csv 
  - > export csv filename.csv
  - > export csv e:\filename.csv
- xml 
  - > export xml filename.xml
### import 
Example:
- csv
```sh
> import csv d:\data\records.csv
10000 records were imported from d:\data\records.csv.

> import csv d:\data\records2.csv
Import error: file d:\data\records.csv is not exist.
```
- xml
```sh
> import xml c:\users\myuser\records.xml
5000 records were imported from c:\users\myuser\records.xml.
```

### purge
Пример использования:

```sh
> purge
Data file processing is completed: 10 of 100 records were purged.
```
### insert 
Пример использования:

```
> insert (id, firstname, lastname, dateofbirth) values ('1', 'John', 'Doe', '5/18/1986')
> insert (dateofbirth,lastname,firstname,id) values ('5/18/1986','Stan','Smith','2')
```
### delete
Пример использования:

```
> delete where id = '1'
Record #1 is deleted.
> delete where LastName='Smith'
Records #2, #3, #4 are deleted. 
```
### update
Пример использования:

```
> update set firstname = 'John', lastname = 'Doe' , dateofbirth = '5/18/1986' where id = '1'
> update set DateOfBirth = '5/18/1986' where FirstName='Stan' and LastName='Smith'
```

### select 
Пример использования команды:

```
> select id, firstname, lastname where firstname = 'John' and lastname = 'Doe'
+----+-----------+----------+
| Id | FirstName | LastName |
+----+-----------+----------+
|  1 | John      | Doe      |
+----+-----------+----------+
> select lastname where firstname = 'John' or lastname = 'Smith'
+----------+
| LastName |
+----------+
| Doe      |
| Smith    |
+----------+
```

## Command line parameters for FileCabinetGenerator.
| Full Parameter Name | Short Parameter Name | Description                    |
|---------------------|----------------------|--------------------------------|
| output-type         | t                    | Output format type (csv, xml). |
| output              | o                    | Output file name.              |
| records-amount      | a                    | Amount of generated records.   |
| start-id            | i                    | ID value to start.             |

Пример использования:

```sh
$ FileCabinetGenerator.exe --output-type=csv --output=d:\data\records.csv --records-amount=10000 --start-id=30
10000 records were written to records.csv.

$ FileCabinetGenerator.exe -t xml -o c:\users\myuser\records.xml -a 5000 -i 45
5000 records were written to c:\users\myuser\records.xml
```
