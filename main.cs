FAST LOOP {
	COLLECT DATA;
	PROCESS DATA{
		LOTS OF STUFF TO BE ADDED HERE;
	}
	STORE DATA;
	IF (COLOR TRANSITION LEVEL REACHED){
			SEND COLOR LEVELS TO DISPLAY;
		}
}

SLOW LOOP {
	PERIODICALLY SCAN COMMUNICATION CASES{ //optimal frequency to be determined and can be updated in settings.
		IF (DISPLAY CONNECTION EXISTS){
			Keep connection warm;
		}
		IF (HIGHER PIORITY CONNECTION EXISTS){
			INTERRUPT AND SWITCH
		}
	}
}

	CONNECT TO DISPLAY;





[WIRELESS COMMUNICATION MODES]{
	CASES (in order of priority):
		[CONTROLLER-ANALYST CONNECTION]: 
			//Data sync takes precedence over all other in order to minimize upload time of all data that hasn't been sync'd yet. 
		[CONTROLLER AS ACCESS POINT]: 
			//Controller acts as an access point when triggered by button press (currently on Controller, but preferably on Display due to easier and safer access). 
		[CONTROLLER-DISPLAY CONNECTION]: 
			//Only needs to be established when color levels change. As it is now, Display acts as a dedicated access point for Controller. Connection should be quick whenever Controller chooses to send transition. This connection takes precedence over all others except for analyst connection.
			Currently the Display broadcasts the MAC address of the Controller as its SSID. When that SSID is present, the Controller can easily connect to the display and communicate color transitions. 
			
				
		[CONTROLLER-CLOUD CONNECTION]: 
			//Connect periodically at an optimal frequency. Could be anywhere from every 3 minutes to once a day depending on how long it takes to upload new data cache.
				Can be interupted by color transition to maintain real time household feedback.
		[TRIGGER CONNECTION]: 
			//Undecided if this would be helpful for anything. Possibly an external trigger for things like entering pairing mode without having to open breaker box, or trigging [CONTROLLER AS ACCESS POINT]. This wouldn't be a true communication mode and maybe should be specified elsewhere. 
}

[SCAN FOR AVENUES OF COMMUNICATION]{
	if exist(initialization_SSID || analyst_SSID || local_SSID){
		[CONNECT TO LOCAL ACCESS POINT];
		[CONNECT TO SERVER];
			priority given to initialization_SSID, analyst_SSID, local_SSID in that order;
	}
}


[ESTABLISH CONNECTION WITH SERVER]{
	[CONNECT TO LOCAL ACCESS POINT];
	[CONNECT TO SERVER];
	[HANDSHAKE AND IDENTIFY];
}



[CONNECT TO SERVER]{
	
}


[HANDSHAKE AND IDENTIFY]{
	MAKE CONNECTION;
	DEVICE SENDS MAC TO SERVER;
	SERVER QUERIES DATABASE SPECIFIC TO DEVISE MAC ADDRESS]
}

[SYNC SETTINGS]{
	COMPARE CHANGED SETTINGS TABLES; //



[COMPARE CHANGED SETTINGS TABLE]{
	
}