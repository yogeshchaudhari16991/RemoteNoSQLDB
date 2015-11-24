

###This is a readme file for RemoteNoSQLDatabase:###
=======================================================

###Compile and run the code:
-------------------------------
1. Compile the code using compile.bat file
2. To run the code please run run.bat file
   This file starts executing the RemoteNoSQLDatabase.exe file.
   RemoteNoSQLDatabase.exe file takes two arguments 
i.  Number of Read Clients to start
ii. Number of Write Clients to start
   (By default both are set to 2)

###Using the RemoteNoSQLDatabase Application:

------------------------------------------------
3. When run.bat is executed, it automatically starts given number of read and write clients and also server and WPF client. 
4. Read and write clients will start sending 1000 messages to server and will receive reply for those messages from sender.
5. On WPF client before doing any work, first connect to the server on port 8080 using "start" button in "connect" tab.

6. Then press "test-result" button on WPF client in "connect" tab. This will write the writer client's average message processing time,
   reader client's average latency and server's average throuput in the listbox below.

7. Using 2nd tab in WPF client i.e. DBOperations TA and instructor can manually check if the database operations and queries can be
   remotely done or not.
   Note: (Due to stress testing going on in other consoles, sometimes server takes few seconds time to respond to WPF clients requests.)
   Note: (while making queries the Timestamp Field value, Start Date field value and End Date field values has to be in
		  MM/DD/YYYY HH:MM:SS AM/PM format)
		 (The default given values in text fileds of key, children, payload, file name, pattern etc. won't execute, user has to change them.)
			
8. User can select which database he wants to make DBOperations and queries on in database list of 2nd tab. The corresponding associated queries
   with that type of database will apear in query list.
   
9. While using Persist Database query, if the file given does not exist or all the keys of elements present in database are same as those that
   can be augmented from given input XMl file the response message will be "not able to restore database from given file".
   For exact reason TA and instructor can look at server console, where it will say if it was due to file does not exist or other reason.
   
10. If TA or instructor wants to change the queries those are made by writer and reader clients, they can change them from read1.xml file and 
	write1.xml file for respective clients.
	Note: (The structure for all types of queries both clients support is already given in read.xml and write.xml file for respective clients.)
	
### Location of all required files:
------------------------------------
read.xml: Read Client/bin/Debug/read.xml	
read1.xml: Read Client/bin/Debug/read1.xml
write.xml: Write Client/bin/Debug/write.xml
write1.xml: Write Client/bin/Debug/write1.xml
enum_db.xml: Server/bin/Debug/enum_db.xml
db.xml: Server/bin/Debug/db.xml
test.xml: Server/bin/Debug/test.xml
