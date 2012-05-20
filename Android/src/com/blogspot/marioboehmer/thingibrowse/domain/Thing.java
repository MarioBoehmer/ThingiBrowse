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
import java.util.List;
import java.util.Map;

/**
 * Domain object to represent a Thing's details.
 * 
 * @author Mario Böhmer
 */
public class Thing implements Serializable {

	private static final long serialVersionUID = 5493150793606114044L;

	private String thingTitle;
	private String thingCreatedBy;
	private String thingCreatorImageUrl;
	private String thingDate;
	private String thingDescription;
	private String thingCreatorUrl;
	private String thingImageUrl;
	private String thingLargeImageUrl;
	private String thingInstructions;
	private Map<String, String[]> thingFiles;
	private List<String> thingAllImageDetailPageUrls;

	public Thing(String thingTitle, String thingCreatedBy,
			String thingCreatorImageUrl, String thingDate,
			String thingDescription, String thingCreatorUrl,
			String thingImageUrl, String thingLargeImageUrl,
			String thingInstructions, Map<String, String[]> thingFiles,
			List<String> thingAllImageDetailPageUrls) {
		this.thingTitle = thingTitle;
		this.thingCreatedBy = thingCreatedBy;
		this.thingCreatorImageUrl = thingCreatorImageUrl;
		this.thingDate = thingDate;
		this.thingDescription = thingDescription;
		this.thingCreatorUrl = thingCreatorUrl;
		this.thingImageUrl = thingImageUrl;
		this.thingLargeImageUrl = thingLargeImageUrl;
		this.thingInstructions = thingInstructions;
		this.thingFiles = thingFiles;
		this.thingAllImageDetailPageUrls = thingAllImageDetailPageUrls;
	}

	public String getThingTitle() {
		return thingTitle;
	}

	public String getThingCreatedBy() {
		return thingCreatedBy;
	}

	public String getThingCreatorImageUrl() {
		return thingCreatorImageUrl;
	}

	public String getThingDate() {
		return thingDate;
	}

	public String getThingDescription() {
		return thingDescription;
	}

	public String getThingCreatorUrl() {
		return thingCreatorUrl;
	}

	public String getThingImageUrl() {
		return thingImageUrl;
	}

	public String getThingLargeImageUrl() {
		return thingLargeImageUrl;
	}

	public String getThingInstructions() {
		return thingInstructions;
	}

	public Map<String, String[]> getThingFiles() {
		return thingFiles;
	}

	public List<String> getThingAllImageDetailPageUrls() {
		return thingAllImageDetailPageUrls;
	}
}
