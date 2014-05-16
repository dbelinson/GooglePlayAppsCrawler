GooglePlayStoreCrawler
======================

Simple scalable crawler for Apps data from the Play Store.

You don't have to input any of your Google Account credentials since this Crawler acts like a "Logged Out" user.

# About me
My name is Marcello Lins, i am a 23 y/o developer from Brazil who works with BigData and DataMining techniques at the moment.

http://about.me/marcellolins

# What is this project about ? 

The main idea of this project is to gather/mine data about apps of the Google Play Store and build a rich database so that developers, android fans and anyone else can use to generate statistics about the current play store situation

There are many questions we have no answer at the moment and we should be able to answer than with this database.

# What do i need before i start?

* I highly recommend you read all the pages of this wiki, which won`t take long.

* Know C#

# How about the database?

* I have made my MongoDB database public, including a user with read/write permissions so we can all use and populate the same database

* If you feel like, you can make your own MongoDB Database and change the code Consts to point the output to your own MongoDB Database. No Biggie


**Refer to the Pages section of this wiki for individual information about each aspect of the project.**

<form action="https://www.paypal.com/cgi-bin/webscr" method="post" target="_top">
<input type="hidden" name="cmd" value="_s-xclick">
<input type="hidden" name="encrypted" value="-----BEGIN PKCS7-----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-----END PKCS7-----
">
<input type="image" src="https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif" border="0" name="submit" alt="PayPal - The safer, easier way to pay online!">
<img alt="" border="0" src="https://www.paypalobjects.com/en_US/i/scr/pixel.gif" width="1" height="1">
</form>
