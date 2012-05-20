/*   Copyright 2012 Mario Böhmer
 *
 *   Licensed under Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported (CC BY-NC-SA 3.0) 
 *   you may not use this file except in compliance with the License.
 *   You may obtain a copy of the License at
 *
 *       http://creativecommons.org/licenses/by-nc-sa/3.0/
 */
package com.blogspot.marioboehmer.thingibrowse.domain;

import java.io.Serializable;

import android.widget.ListView;

import com.blogspot.marioboehmer.thingibrowse.fragments.ThingResultListFragment;

/**
 * Domain object to represent an item in the {@link ThingResultListFragment}'s
 * {@link ListView}.
 * 
 * @author Mario Böhmer
 */
public class ThingResultListItem implements Serializable {

	private static final long serialVersionUID = -3964683443697700389L;

	private String thingTitle;
	private String thingCreatedBy;
	private String thingTime;
	private String thingUrl;
	private String thingCreatorUrl;
	private String thingImageUrl;

	public ThingResultListItem(String thingTitle, String thingCreatedBy,
			String thingTime, String thingUrl, String thingCreatorUrl,
			String thingImageUrl) {
		this.thingTitle = thingTitle;
		this.thingCreatedBy = thingCreatedBy;
		this.thingTime = thingTime;
		this.thingUrl = thingUrl;
		this.thingCreatorUrl = thingCreatorUrl;
		this.thingImageUrl = thingImageUrl;
	}

	public String getThingTitle() {
		return thingTitle;
	}

	public String getThingCreatedBy() {
		return thingCreatedBy;
	}

	public String getThingTime() {
		return thingTime;
	}

	public String getThingUrl() {
		return thingUrl;
	}

	public String getThingCreatorUrl() {
		return thingCreatorUrl;
	}

	public String getThingImageUrl() {
		return thingImageUrl;
	}

}
