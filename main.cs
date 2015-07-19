FAST LOOP {
	COLLECT DATA;{
		Interlace ADC signal polling of sensors at 12 KSPS //Quick calculations suggest that this is a high enough sample rate, but this should be critically assessed. Atmel chip is capable of 320 KSPS on a single channel, but apparently can interleave if channels share bandwidth.
		Keep fast sampling in a dedicated portion of cache, as large of a contiguous dataset as allowed.
		Record higher sample rate
	}
	PROCESS DATA{
		LOTS OF STUFF TO BE ADDED HERE;
		
		[Calculate true RMS current and phase angle]
		[Use a change rate algorithim to determine which samples to record]{ // we should have two or three sample rates. 
			1. fast enough to encompass all the most statistically relevant aspect of the power signature in the first moments of turning on. Roughly guessing 10 samples per second.
			2. an intermediate rate for power transitions that change in a longer time frame when relevant signature vectors can be determined with lower resolution. // i.e. if some fictitious appliance pulls heavy current in the first second but whose power needs slowly fall off over many seconds, then we can get our relvant signature vector from a second by second data storage. 
			3. a slow sample rate for when nothing is really going on. If no classification significant power changes are being detected, choose the slowest possible sample rate. Our slow sample rate algorithm could be dynamically set so that so that energy calculations are accurate when power curve is recalulated. // for instance if nothing significant happens in the time frame that completely fills our SD write cache (44 seconds), why not reduce that period to one timestamp and one power value that reproduces the exact energy consumed during that 44 seconds (or however long it takes before a faster VSR (variable sample rate)is needed. 
			
						
		}
		[Collect VSR samples in dedicated portion of cache that matches the size of an SD sector or two.]
		
	}
	

}

SLOW LOOP {
	PERIODICALLY SCAN COMMUNICATION CASES{
		//optimal frequency to be determined and can be updated in settings.
		IF (DISPLAY CONNECTION EXISTS){
			Keep connection warm;
		}
		IF (HIGHER PIORITY CONNECTION EXISTS){
			INTERRUPT AND SWITCH
		}
	}
	IF (COLOR TRANSITION LEVEL REACHED){
		SEND COLOR LEVELS TO DISPLAY;
	}
	CONNECT TO DISPLAY;{
		
	}
	[STORE VSR DATA]{
		//When samples cache is full write to SD memory
		Timestamp technique{
			Currently, we record one timestamp every 44 seconds, knowing that a sector fills in 44 seconds, we can know all timestamps with only one timestamp per sector. With VSR (variable sample rate), we have to somehow keep track of sample times.
			1. write a timestamp for every sample. Simple but seems like a waste of storage media.
			2. write an iteger for elapsed time since last time stamp for every sample. Just write a full timestamp often enough so that the elapse integer is smaller than the actual time stamp.
			3. the most efficient I've though of so far is a timestamp at the start of every VSR change along with a VSR rate ID. If we have three VSR rates (i.e. 1/10 S, 1 S, or 44 S) with three specific identifiers (2-bits) expected at the prefix or suffic of every timestamp, then we can faithfully reconstruct the power signal by simple counting the number of samples between each timestamp. Better, if it's not too difficult to add the extra conditional when decoding, if a slow VSR identifier is used, the next byte provides the total seconds for that single sample, making 256 seconds the limit on the slowest sample rate possible.
		}
	}
	
	PROCESS DATA FOR DISAGGREGATION{
		It's probably entirely possible to keep all dissaggregation processing on the servers. Is there any benefit to classification from the device? Hard processing should be kept at a minumum. However, if a highly probably vector space signature has been determined on the server, it probably does not take much processing to calculate whether a power event matches an existing set of vector spaces. A likely filtering algorithm might have the most significant component of the vector space first in the list, narrowing down matches for then next most significant component.
		For example, say three signatures are defined by the following in order of significance{
			Sig1
			* a 4.2 Amp load difference within 1/2 a second.
			* a drop of 1.6 Amps within a 1.5 seconds.
			* a phase angle of x within 1/10 second.
			* a low amperage cyclic pulse that continues for the duration after 2 seconds.
			Sig2
			* a 4.2 Amp load difference within 1/2 a second.
			* a slow stepwise ramp down of 1.2 Amps over 4 seconds.
			* a phase angle of y every 1/2 second during ramp down.
			Sig3
			* a 1.4 Amp load difference within 1/10 a second.
			* a bump up to an additional 2 Amps at 1 second.
		}
		If we had 20 different appliances identified, we would run a correlation of the most significan component for all 10 over the power event. If the power event matches 4.2 Amps within 1/2 second, the algorithm continues the correlation with only the two matching signatures and so forth.
		If identification is successful, a signature id is recorded with the timestamp, or in a separate table.
		If identification fails, the server can simply filter all the events by missing ID.
		It would be nice if our processor could handle a local unidentified events table that was managed based on very simple rules, like a vector space comprised of just the first local maxima of current demand and phase angle. Then all similar events could quickly grouped.
		The question is whether there is any need for our system to report event identification locally. If we are providing an API to assist an ecosystem of smart home controls, do we want to have to rely 100% on central servers to report an event identification.
		I see a balance of using the servers for the classification of our events and isolating the most probably signature components. But once those are determined, let the DEVICE report the indentifiable events.
		Also, I suspect that the classification algorithm will be light weight enough to include in our app. If our app is capable of interacting directly with our devices, then contributing to the ecosystem could be independent of the servers. I also suspect that requiring classification on the servers with provide us much more rich data to manage the industry with. I fine balance would be to require classification on our servers, but allow identification of pre-classified events locally. That way things will continue to work well even when central connection is compromised. 
		
	}
}

[WIRELESS COMMUNICATION MODES]{
	CASES (in order of priority):{
		[CONTROLLER-ANALYST CONNECTION]:{
			//Data sync takes precedence over all other in order to minimize upload time of all data that hasn't been sync'd yet. 
		}
		[CONTROLLER AS ACCESS POINT]:{
			//Controller acts as an access point when triggered by button press (currently on Controller, but preferably on Display due to easier and safer access). 
		}
		[CONTROLLER-DISPLAY CONNECTION]:{
			//Only needs to be established when color levels change. As it is now, Display acts as a dedicated access point for Controller. Connection should be quick whenever Controller chooses to send transition. This connection takes precedence over all others except for analyst connection.
			Currently the Display broadcasts the MAC address of the Controller as its SSID. When that SSID is present, the Controller can easily connect to the display and communicate color transitions. 
		}
		[CONTROLLER-CLOUD CONNECTION]:{
			//Connect periodically at an optimal frequency. Could be anywhere from every 3 minutes to once a day depending on how long it takes to upload new data cache.
				Can be interupted by color transition to maintain real time household feedback.
		}
		[TRIGGER CONNECTION]:{
			//Undecided if this would be helpful for anything. Possibly an external trigger for things like entering pairing mode without having to open breaker box, or trigging [CONTROLLER AS ACCESS POINT]. This wouldn't be a true communication mode and maybe should be specified elsewhere.
		}
	}
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

[UPDATE SETTINGS]{ //process should be similar on both SERVER or DEVICE.
	If changes detected on user interface settings pages{
		[Uptate respective changes in Local parameters table]
		[Update timestamp in local CHANGED_SETTINGS_TABLE]
		[Set local update_flag] //to be read next SERVER/DEVICE sync
	}
}

[SYNC SETTINGS]{
	IF SERVER or DEVICE uptade_flag is set{
		[Compare settings timestamps and sync all parameters to any with a newer change timestamp]
		[clear update_flags upon completion]
	}
}

[DISAGGREGATION]{
	DEVICE SIDE{
		CHANGE DETECTED IN SIGNAL{
			Does change statistically match previously identified power signatures?{
				NOTE: This needs it's own DB table
				YES{
					RECORD SIGNATURE ID AND TIMESTAMP
				}
				NO{
					CREATE NEW SIGNATURE ID ENTRY
					RECORD SIGNATURE ID AND TIMESTAMP
				}
			}
		}
	}
	SERVER SIDE{
		[CLOUD SERVER PROCESS]{
			Add new signature records to statistical pool, strengthening identification vector space or for charting appliance signiture changes over time (wearing out / product lifetime prediction).
			Once calculated, update signature vector spaces table for continually improved DEVICE side classification of existing and new power signatures.
			}
		}
	}	


