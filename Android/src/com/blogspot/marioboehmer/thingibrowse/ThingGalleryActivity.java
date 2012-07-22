/*   Copyright 2012 Mario Böhmer
 *
 *   Licensed under Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported (CC BY-NC-SA 3.0) 
 *   you may not use this file except in compliance with the License.
 *   You may obtain a copy of the License at
 *
 *       http://creativecommons.org/licenses/by-nc-sa/3.0/
 */
package com.blogspot.marioboehmer.thingibrowse;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.AsyncTask;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Gallery;
import android.widget.ProgressBar;

import com.actionbarsherlock.app.SherlockActivity;
import com.actionbarsherlock.view.Menu;
import com.actionbarsherlock.view.MenuItem;
import com.blogspot.marioboehmer.thingibrowse.adapter.ThingGalleryAdapter;
import com.blogspot.marioboehmer.thingibrowse.network.ThingRequester;
import com.blogspot.marioboehmer.thingibrowse.network.ThingiverseHTMLParser;

/**
 * Used to display a gallery view of all attached thing images.
 * 
 * @author Mario Böhmer
 */
public class ThingGalleryActivity extends SherlockActivity {

	private static final String TAG = ThingGalleryActivity.class
			.getSimpleName();
	private static final String IMAGE_URLS = "imageUrls";
	private static final String IMAGE_DETAIL_PAGE_URLS = "imageDetailPageUrls";
	private static final int CONNECTIVITY_MANAGER_TYPE_ETHERNET_COMPATIBILITY = 9;

	private Gallery gallery;
	private ProgressBar progressBar;

	private String[] imageDetailPageUrls;
	private String[] imageUrls;

	private boolean loadLargeImages = false;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		getSupportActionBar().setDisplayShowTitleEnabled(false);
		getSupportActionBar().setDisplayHomeAsUpEnabled(true);

		setContentView(R.layout.thing_gallery_activity);
		progressBar = (ProgressBar) findViewById(android.R.id.empty);
		gallery = (Gallery) findViewById(R.id.gallery);

		int currentScreenWidth = getWindowManager().getDefaultDisplay()
				.getWidth();
		loadLargeImages = (!isFastNetwork() || currentScreenWidth < 640) ? false
				: true;

		if (savedInstanceState == null) {
			savedInstanceState = getIntent().getExtras();
		}
		if (savedInstanceState != null) {
			imageUrls = savedInstanceState.getStringArray(IMAGE_URLS);
			imageDetailPageUrls = savedInstanceState
					.getStringArray(IMAGE_DETAIL_PAGE_URLS);

			if (imageUrls != null) {
				showImages(Arrays.asList(imageUrls));
			} else {
				fetchImageUrls();
			}
		}
	}
	
	private void fetchImageUrls() {
		new AsyncTask<String, Void, List<String>>() {

			@Override
			protected List<String> doInBackground(String... imageDetailUrls) {
				try {
					return ThingRequester.getInstance(getApplicationContext()).getThingImageUrls(imageDetailUrls, loadLargeImages);
				} catch (IOException e) {
					Log.e(TAG, e.toString());
					return new ArrayList<String>();
				}
			}

			protected void onPostExecute(List<String> result) {
				imageUrls = result.toArray(new String[result.size()]);
				showImages(result);
			};

		}.execute(imageDetailPageUrls);
	}

	private boolean isFastNetwork() {
		ConnectivityManager connManager = (ConnectivityManager) getSystemService(CONNECTIVITY_SERVICE);
		NetworkInfo ethernetNetworkInfo = connManager
				.getNetworkInfo(CONNECTIVITY_MANAGER_TYPE_ETHERNET_COMPATIBILITY);
		if (ethernetNetworkInfo != null && ethernetNetworkInfo.isConnected()) {
			return true;
		} else {
			NetworkInfo wifiNetworkInfo = connManager
					.getNetworkInfo(ConnectivityManager.TYPE_WIFI);
			if (wifiNetworkInfo != null && wifiNetworkInfo.isConnected()) {
				return true;
			}
		}
		return false;
	}

	private void showImages(List<String> imageUrls) {
		ThingGalleryAdapter adapter = new ThingGalleryAdapter(this);
		adapter.setImageUrls(imageUrls);
		adapter.notifyDataSetChanged();
		gallery.setAdapter(adapter);
		progressBar.setVisibility(View.GONE);
		gallery.setVisibility(View.VISIBLE);
	}

	@Override
	protected void onSaveInstanceState(Bundle outState) {
		outState.putStringArray(IMAGE_DETAIL_PAGE_URLS, imageDetailPageUrls);
		outState.putStringArray(IMAGE_URLS, imageUrls);
		super.onSaveInstanceState(outState);
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		super.onCreateOptionsMenu(menu);
		getSupportMenuInflater().inflate(R.menu.details_and_gallery_menu, menu);
		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		switch (item.getItemId()) {
		case android.R.id.home:
			startActivity(ActionBarHelper.getInstance().getHomeIntent(
					getApplicationContext()));
			return true;
		case R.id.feedback:
			startActivity(ActionBarHelper.getInstance().getFeedbackIntent(
					getApplicationContext()));
			return true;
		case R.id.info:
			startActivity(ActionBarHelper.getInstance().getInfoIntent(
					getApplicationContext()));
			return true;
		default:
			return super.onOptionsItemSelected(item);
		}
	}
}
