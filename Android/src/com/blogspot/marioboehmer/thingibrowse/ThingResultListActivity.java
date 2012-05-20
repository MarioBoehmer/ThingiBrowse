/*   Copyright 2012 Mario Böhmer
 *
 *   Licensed under Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported (CC BY-NC-SA 3.0) 
 *   you may not use this file except in compliance with the License.
 *   You may obtain a copy of the License at
 *
 *       http://creativecommons.org/licenses/by-nc-sa/3.0/
 */
package com.blogspot.marioboehmer.thingibrowse;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Toast;

import com.actionbarsherlock.app.ActionBar;
import com.actionbarsherlock.app.ActionBar.OnNavigationListener;
import com.actionbarsherlock.app.SherlockFragmentActivity;
import com.actionbarsherlock.view.Menu;
import com.actionbarsherlock.view.MenuItem;
import com.blogspot.marioboehmer.thingibrowse.fragments.ThingDetailsFragment;
import com.blogspot.marioboehmer.thingibrowse.fragments.ThingResultListFragment;
import com.blogspot.marioboehmer.thingibrowse.fragments.ThingResultListFragment.OnThingSelectedListener;
import com.blogspot.marioboehmer.thingibrowse.network.ThingRequester.ThingResultListType;

/**
 * Depending on layout capabilities of the device, this {@link Activity} can
 * display either just the {@link ThingResultListFragment} on small devices or
 * the {@link ThingResultListFragment} and the {@link ThingDetailsFragment} side
 * by side on large devices.
 * 
 * @author Mario Böhmer
 */
public class ThingResultListActivity extends SherlockFragmentActivity implements
		OnThingSelectedListener, OnNavigationListener, OnNetworkErrorListener {

	private static final String POPULAR_THINGS = "Popular Things";
	private static final String NEWEST_THINGS = "Newest Things";

	private static final int NEWEST_THINGS_NAV_POSITION = 0;
	private static final int POPULAR_THINGS_NAV_POSITION = 1;
	private int currentNavPosition = NEWEST_THINGS_NAV_POSITION;

	private ThingDetailsFragment thingDetailsFragment;
	private ThingResultListFragment thingResultListFragment;

	public boolean isDualFragmentsLayout;
	private boolean wasNetworkErrorShown = false;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		getSupportActionBar().setDisplayShowTitleEnabled(false);
		ArrayAdapter<String> adapter = new ArrayAdapter<String>(this,
				R.layout.sherlock_spinner_item, new String[] { NEWEST_THINGS,
						POPULAR_THINGS });
		adapter.setDropDownViewResource(R.layout.sherlock_spinner_dropdown_item);
		getSupportActionBar().setListNavigationCallbacks(adapter, this);
		getSupportActionBar().setNavigationMode(ActionBar.NAVIGATION_MODE_LIST);

		setContentView(R.layout.thing_result_list_activity);
		thingDetailsFragment = (ThingDetailsFragment) getSupportFragmentManager()
				.findFragmentById(R.id.thingDetailsFragment);
		thingResultListFragment = (ThingResultListFragment) getSupportFragmentManager()
				.findFragmentById(R.id.thinglistFragment);
		if (thingDetailsFragment != null && thingDetailsFragment.isInLayout()) {
			isDualFragmentsLayout = true;
		}

		if (savedInstanceState != null) {
			currentNavPosition = savedInstanceState
					.getInt("currentNavPosition");
			getSupportActionBar().setSelectedNavigationItem(currentNavPosition);
		}
	}

	@Override
	protected void onResume() {
		super.onResume();
		wasNetworkErrorShown = false;
	}

	@Override
	public void onThingSelected(String thingUrl) {
		if (isDualFragmentsLayout) {
			thingDetailsFragment.loadThing(thingUrl);
		} else {
			Bundle thingBundle = new Bundle();
			thingBundle.putString("thingUrl", thingUrl);
			Intent thingDetailsIntent = new Intent(getApplicationContext(),
					ThingDetailsActivity.class);
			thingDetailsIntent.putExtras(thingBundle);
			startActivity(thingDetailsIntent);
			onSaveInstanceState(thingBundle);
		}
	}

	@Override
	protected void onSaveInstanceState(Bundle outState) {
		outState.putInt("currentNavPosition", currentNavPosition);
		super.onSaveInstanceState(outState);
	}

	@Override
	public boolean onNavigationItemSelected(int itemPosition, long itemId) {
		if (itemPosition == NEWEST_THINGS_NAV_POSITION
				&& itemPosition != currentNavPosition) {
			thingResultListFragment
					.resetFragmentWithThingResultListType(ThingResultListType.NEWEST_THINGS);
		} else if (itemPosition == POPULAR_THINGS_NAV_POSITION
				&& itemPosition != currentNavPosition) {
			thingResultListFragment
					.resetFragmentWithThingResultListType(ThingResultListType.POPULAR_THINGS);
		}
		currentNavPosition = itemPosition;
		return true;
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		super.onCreateOptionsMenu(menu);
		getSupportMenuInflater().inflate(R.menu.menu, menu);
		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		switch (item.getItemId()) {
		case android.R.id.home:
			startActivity(ActionBarHelper.getInstance().getHomeIntent(
					getApplicationContext()));
			return true;
		case R.id.refresh:
			thingResultListFragment.refresh();
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

	@Override
	public void onNetworkError() {
		View progressBar = findViewById(android.R.id.empty);
		if (progressBar != null && progressBar.isShown()) {
			progressBar.setVisibility(View.GONE);
		}
		if (!wasNetworkErrorShown) {
			runOnUiThread(new Runnable() {

				@Override
				public void run() {
					Toast.makeText(ThingResultListActivity.this,
							getString(R.string.network_error),
							Toast.LENGTH_LONG).show();
				}
			});
			wasNetworkErrorShown = true;
		}

	}
}