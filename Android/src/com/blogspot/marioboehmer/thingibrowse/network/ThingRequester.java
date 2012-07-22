/*   Copyright 2012 Mario Böhmer
 *
 *   Licensed under Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported (CC BY-NC-SA 3.0) 
 *   you may not use this file except in compliance with the License.
 *   You may obtain a copy of the License at
 *
 *       http://creativecommons.org/licenses/by-nc-sa/3.0/
 */
package com.blogspot.marioboehmer.thingibrowse.network;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.ArrayList;
import java.util.List;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.util.Log;

import com.blogspot.marioboehmer.thingibrowse.domain.Thing;
import com.blogspot.marioboehmer.thingibrowse.domain.ThingResultListItem;

/**
 * Handles the HTTP requests to the thingiverse platform and returns the raw
 * HTML for the corresponding request.
 * 
 * @author Mario Böhmer
 */
public class ThingRequester {

	private static final String TAG = ThingRequester.class.getSimpleName();
	private static ThingRequester INSTANCE;

	private ThingRequester() {

	}

	public static ThingRequester getInstance(Context context)
			throws IOException {
		if (INSTANCE == null) {
			INSTANCE = new ThingRequester();
		}
		if (!isNetworkAvailable(context)) {
			throw new IOException("Network not Available.");
		}
		return INSTANCE;
	}

	public Thing getThing(String url) throws ThingException {
		Thing thing = null;
		try {
			thing = ThingiverseHTMLParser.getThing(getResponseHtml(url));
			if (thing == null) {
				throw new ThingException();
			}
		} catch (Exception e) {
			throw new ThingException();
		}
		return thing;
	}

	public ArrayList<ThingResultListItem> getThingResultList(String html,
			boolean isSearch) throws ThingException {
		ArrayList<ThingResultListItem> thingResultList = null;
		try {
			if (isSearch) {
				thingResultList = ThingiverseHTMLParser
						.getThingResultListItemsForSearch(html);
			} else {
				thingResultList = ThingiverseHTMLParser
						.getThingResultListItems(html);
			}
			if (thingResultList == null) {
				throw new ThingException();
			}
		} catch (Exception e) {
			throw new ThingException();
		}
		return thingResultList;
	}

	public int getThingResultListLastPageIndex(String html, boolean isSearch)
			throws ThingException {
		int lastPageIndex = -1;
		try {
			if (isSearch) {
				lastPageIndex = ThingiverseHTMLParser
						.getThingsLastPageIndexForSearch(html);
			} else {
				lastPageIndex = ThingiverseHTMLParser
						.getThingsLastPageIndex(html);
			}
		} catch (Exception e) {
			throw new ThingException();
		}
		return lastPageIndex;
	}

	public List<String> getThingImageUrls(String[] imageDetailUrls,
			boolean loadLargeImages) {
		List<String> imageUrls = new ArrayList<String>();
		if (imageDetailUrls != null) {
			for (int x = 0; x < imageDetailUrls.length; x++) {
				String imageResponseHtml = null;
				try {
					imageResponseHtml = getResponseHtml(imageDetailUrls[x]);
				} catch (IOException e) {
					Log.e(TAG, e.toString());
				} catch (Exception e) {
					Log.e(TAG, e.toString());
				}
				if (imageResponseHtml != null) {
					String imageUrl = null;
					if (loadLargeImages) {
						imageUrl = ThingiverseHTMLParser
								.getLargeImageUrl(imageResponseHtml);
					} else {
						imageUrl = ThingiverseHTMLParser
								.getMediumImageUrl(imageResponseHtml);
					}
					if (imageUrl != null) {
						imageUrls.add(imageUrl);
					}
				}
			}
		}
		return imageUrls;
	}

	public String getResponseHtmlForThingResultList(
			String thingCategoryBaseUrl, int pageNumber) throws Exception {
		StringBuilder requestUrl = new StringBuilder();
		requestUrl.append(thingCategoryBaseUrl);
		requestUrl.append("/page:");
		requestUrl.append(pageNumber);
		return getResponseHtml(requestUrl.toString());
	}

	public String getResponseHtmlForSearchThingResultList(int pageNumber,
			String searchTerm) throws Exception {
		StringBuilder requestUrl = new StringBuilder();
		requestUrl.append("http://www.thingiverse.com/search");
		requestUrl.append("/page:");
		requestUrl.append(pageNumber);
		requestUrl.append("?q=");
		requestUrl.append(searchTerm);
		return getResponseHtml(requestUrl.toString());
	}

	public String getResponseHtml(String requestUrl) throws Exception {
		String responseHtml = null;
		try {
			URL url = new URL(requestUrl);
			HttpURLConnection con = (HttpURLConnection) url.openConnection();
			responseHtml = readStream(con.getInputStream());
			con.disconnect();
		} catch (Exception e) {
			Log.e(TAG, e.toString());
			throw e;
		}
		return responseHtml;
	}

	private String readStream(InputStream in) throws Exception {
		BufferedReader reader = null;
		StringBuilder stringBuilder = new StringBuilder();
		try {
			reader = new BufferedReader(new InputStreamReader(in));
			String line = "";
			while ((line = reader.readLine()) != null) {
				stringBuilder.append(line);
			}
		} catch (IOException e) {
			Log.e(TAG, e.toString());
			throw e;
		} finally {
			if (reader != null) {
				try {
					reader.close();
				} catch (IOException e) {
					Log.e(TAG, e.toString());
				}
			}
		}
		return stringBuilder.toString();
	}

	private static boolean isNetworkAvailable(Context context) {
		ConnectivityManager cm = (ConnectivityManager) context
				.getSystemService(Context.CONNECTIVITY_SERVICE);
		NetworkInfo networkInfo = cm.getActiveNetworkInfo();
		if (networkInfo != null && networkInfo.isConnected()) {
			return true;
		}
		return false;
	}
}