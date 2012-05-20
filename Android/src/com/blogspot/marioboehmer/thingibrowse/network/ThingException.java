/*   Copyright 2012 Mario Böhmer
 *
 *   Licensed under Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported (CC BY-NC-SA 3.0) 
 *   you may not use this file except in compliance with the License.
 *   You may obtain a copy of the License at
 *
 *       http://creativecommons.org/licenses/by-nc-sa/3.0/
 */
package com.blogspot.marioboehmer.thingibrowse.network;

import com.blogspot.marioboehmer.thingibrowse.domain.Thing;
import com.blogspot.marioboehmer.thingibrowse.domain.ThingResultListItem;

/**
 * Used to describe that there was a problem while parsing the {@link Thing} or
 * the {@link ThingResultListItem} data.
 * 
 * @author Mario Böhmer
 */
public class ThingException extends Exception {

	private static final long serialVersionUID = 3482162735797453354L;

	@Override
	public String getMessage() {
		return "Error parsing the Thing data.";
	}
}
