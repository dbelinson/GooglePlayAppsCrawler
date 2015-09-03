GooglePlayStoreCrawler
======================

Simple scalable crawler for Apps data from the Play Store.

For you Pythonistas out there, here's the <a href="https://github.com/MarcelloLins/GooglePlayAppsCrawler.py">Python port of this project</a>

You don't have to input any of your Google Account credentials since this Crawler acts like a "Logged Out" user.

New: Donate-Button
======================

<a href='https://pledgie.com/campaigns/28270'><img alt='Click here to lend your support to: Google Play Store in Numbers - Open Source Crawler and make a donation at pledgie.com !' src='https://pledgie.com/campaigns/28270.png?skin_name=chrome' border='0' ></a>

Every 100 USD received in donations, I will run a new full-capture of the Play Store and update both the live and the exported versions of the database. 

The costs related to this project range from Virtual Machines costs, paid HTTP-Proxies and storage.

Make it happen !

Important Updates:
---------------------------------------------------------------------------

**Update: Huge Milestone for a home project. The database already have 1.3 Million apps.** 

**Update 2: Added Analytics Features - Powered by Keen.IO**

**Update 3: Updated database is up (June 2015) - Including emails, developer sites and physical addresses**

**Update 4: During 2015/02, I ended up having to pay for 170 GB of outbound traffic on my AWS Bill (that translates to nearly 350 downloads of the file, from which I only received 2 donations).**

# Exporting the Database

As people kept requesting me, i decided to export the database on it's current state.
I you want the exported file, get in touch with me at : marcello.grechi@gmail.com, and I will send you the file. (I decided not to keep the file public, because there were people downloading the same file 
over and over again, and not paying for it, which led to a huge AWS bill that I had to pay).

Have in mind that downloading the database costs me money, since i pay for the outbound traffic provided by AWS when you query the database
So, consider making a donation (via paypal) to marcello.grechi@gmail.com (Pay what you want, Humble Bundle style).

Alternativelly, you can use the "AppsExporter" project that i included to write your own query logic, exporting only the records / fields you need, which will be cheaper than downloading the whole database.

# Opening the Database

Since the exported database contains more than 1 milion lines, you will have to use one text editor such as "EM Editor" for opening the file. Regular text editor such as Notepad++ and Sublime won't open it and will crash.

# About me
My name is Marcello Lins, i am a 24 y/o developer from Brazil who works with BigData and DataMining techniques at the moment.

http://about.me/marcellolins

# What is this project about ? 

The main idea of this project is to gather/mine data about apps of the Google Play Store and build a rich database so that developers, android fans and anyone else can use to generate statistics about the current play store situation

There are many questions we have no answer at the moment and we should be able to answer than with this database.

Here's a <a href="https://s3.amazonaws.com/GooglePlayStore/Google%20Play%20Stats_2015_02.xlsx">simple excel dashboard</a> of data from "2015/02" with some of the answers that we can come up with. 

# What do i need before i start?

* I highly recommend you read all the pages of this wiki, which won't take long.

* Know C#

# How about the database?

* I have made my MongoDB database public, including a user with read/write permissions so we can all use and populate the same database

* If you feel like, you can make your own MongoDB Database and change the code Consts to point the output to your own MongoDB Database. No Biggie


**Refer to the Pages section of this wiki for individual information about each aspect of the project.**
