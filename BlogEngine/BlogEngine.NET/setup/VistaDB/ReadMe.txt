
******************************************************************
Discontinuation of VistaDB Support Notice
******************************************************************

As of BlogEngine.NET 2.0, BlogEngine.NET is no longer supporting VistaDB.  

The files in this VistaDB setup folder are for BlogEngine.NET version 1.6.  You may 
continue to use VistaDB for future versions of BlogEngine.NET by updating your VistaDB 
database to match the latest BlogEngine.NET database schema.  The other database folders 
(e.g. SQLServer, MySQL) contain upgrade scripts for upgrading from BlogEngine.NET 1.6 to 
newer versions.  These upgrade scripts will let you know what changes you need to make in 
your own VistaDB database to keep it current with the latest version of BlogEngine.NET.



******************************************************************
******************************************************************

Running BlogEngine.NET 1.6 using VistaDB:

If you wish to use VistaDB (or VistaDB Express) to store all your blog data, this is 
where you want to be.  Included in this folder are all the scripts that 
you can use to get you started with your blog.  In addition, you will find a sample
web.config file with the needed changes to use VistaDB and an upgrade script for 
current VistaDB users who wish to upgrade from 1.5.

Since the last version of BlogEngine.NET, VistaDB Express is no longer freely available.
If you own a version of VistaDB or have VistaDB Express available, you are fine and will
have an easy setup. If you are already using VistaDB, but no longer have it installed 
and no longer have the installer, you can buy a VistaDB license or convert your data to 
a different free option using the Provider Migration tool. 
(http://www.nyveldt.com/blog/page/BlogEngineNET-Provider-Migration.aspx)

Instructions for new setup:

1. You will need VistaDB or VistaDB Express installed locally.  
2. Find VistaDB.NET20.dll (3.5 Express) or VistaDB4.dll (4.0) on your PC and copy it to your blog's Bin folder. 
3. Copy BlogEngine database file to your blog's App_Data folder. Use the vdb3 extension if you are using 3.5 
or use the vbd4 extension if you are ising version 4.
4. Rename VistaDBWeb.Config to Web.config and copy it to your blog folder.  (This will
overwrite your existing web.config file.  If this is not a new installation, make sure 
you have a backup).  VistaDBWeb.Config is for a .NET 3.5 application pool.  If you will
run BlogEngine.NET in a .NET 4.0 application pool, use VistaDB.NET_4.0_Web.Config instead.
5. Edit your web.config.  Update the connection string and assemblies as needed to 
match your file and version information. The web.config is setup for VistaDB Express 3.5 users.
6. If you are using a non-express version, you will need a license file. Please read this post which covers 
creating a license file to get you going. 

http://www.vistadb.net/blog/post/2009/11/24/Upgrading-Dot-Net-BlogEngine-to-VistaDB-4.aspx

7. Surf out to your Blog and see the welcome post.
8. Login with the username admin and password admin.  Change the password.


Upgrading from 1.5

1. You will need VistaDB or VistaDB Express installed locally.  
2. Open your BlogEngine database in Data Builder and execute the upgrade script against it.  (You will 
likely need to copy your BlogEngine.vdb3(or vbd4) file from your web server, perform the update, and 
copy it back out depending on your setup.
3. The web.config file has changed from 1.5 to 1.6.  It will likely be easiest to start
with the sample web.config file and then make any changes necessary for version numbers, etc.

Note: If you are using a non-express version, please read this post which covers 
creating a license file to get you going. 

http://www.vistadb.net/blog/post/2009/11/24/Upgrading-Dot-Net-BlogEngine-to-VistaDB-4.aspx

Additional information can be found at http://dotnetblogengine.net

Notice:

While BlogEngine.NET is open source and VistaDB Express is free to use, there are a few restrictions.  
VistaDB Express is only free to use for non commercial uses.  If you are commercial, you will need to 
purchase a license to use it.  In addition, the VistaDB Express license requires that you place a link 
back to them in your product.  A link back the vistadb.net in your page footer or side bar would show 
your appreciation.