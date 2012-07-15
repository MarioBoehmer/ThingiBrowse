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
import android.view.View.OnClickListener;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.Toast;

import com.actionbarsherlock.app.ActionBar;
import com.actionbarsherlock.app.ActionBar.OnNavigationListener;
import com.actionbarsherlock.app.SherlockFragmentActivity;
import com.actionbarsherlock.view.Menu;
import com.actionbarsherlock.view.MenuItem;
import com.blogspot.marioboehmer.thingibrowse.fragments.ThingDetailsFragment;
import com.blogspot.marioboehmer.thingibrowse.fragments.ThingResultListFragment;
import com.blogspot.marioboehmer.thingibrowse.fragments.ThingResultListFragment.OnThingSelectedListener;

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

	private static String[] thingsCategoryNames = null;
	private static String[] thingsCategoryBaseUrls = null;
	private int currentNavPosition = 0;

	private ThingDetailsFragment thingDetailsFragment;
	private ThingResultListFragment thingResultListFragment;

	// LeftNavBarButtons
	private Button firstCategoryButton;
	private Button secondCategoryButton;
	private Button thirdCategoryButton;
	private Button fourthCategoryButton;
	private Button refreshButton;
	private Button feedbackButton;
	private Button infoButton;
	private Button[] categoryButtons;

	public boolean isDualFragmentsLayout;
	private boolean wasNetworkErrorShown = false;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		thingsCategoryNames = getResources().getStringArray(
				R.array.things_category_names);
		thingsCategoryBaseUrls = getResources().getStringArray(
				R.array.things_category_base_urls);
		if (ThingiBrowseApplication.isRunningOnGoogleTV()) {
			setContentView(R.layout.thing_result_list_activity_tv);
			setUpLeftNavBar();
		} else {
			setContentView(R.layout.thing_result_list_activity);
			setUpActionBar();
		}
		getSupportActionBar().setDisplayShowTitleEnabled(false);
		getSupportActionBar().setHomeButtonEnabled(false);


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
			if (ThingiBrowseApplication.isRunningOnGoogleTV()) {
				categoryButtons[currentNavPosition].performClick();
			} else {
				getSupportActionBar().setSelectedNavigationItem(
						currentNavPosition);
			}
		}
	}

	@Override
	protected void onResume() {
		super.onResume();
		wasNetworkErrorShown = false;
	}
	
	private void setUpActionBar() {
		ArrayAdapter<String> adapter = new ArrayAdapter<String>(this,
				R.layout.sherlock_spinner_item, thingsCategoryNames);
		adapter.setDropDownViewResource(R.layout.sherlock_spinner_dropdown_item);
		getSupportActionBar().setListNavigationCallbacks(adapter, this);
		getSupportActionBar().setNavigationMode(
				ActionBar.NAVIGATION_MODE_LIST);
	}
	
	private void setUpLeftNavBar() {
		firstCategoryButton = (Button) findViewById(R.id.firstThingCategory);
		secondCategoryButton = (Button) findViewById(R.id.secondThingCategory);
		thirdCategoryButton = (Button) findViewById(R.id.thirdThingCategory);
		fourthCategoryButton = (Button) findViewById(R.id.fourthThingCategory);
		categoryButtons = new Button[] { firstCategoryButton,
				secondCategoryButton, thirdCategoryButton,
				fourthCategoryButton };
		for (Button button : categoryButtons) {
			button.setOnClickListener(categoryOnClickListener);
		}
		
		refreshButton = (Button) findViewById(R.id.refresh_button);
		feedbackButton = (Button) findViewById(R.id.feedback_button);
		infoButton = (Button) findViewById(R.id.info_button);
		Button[] actionButtons = new Button[]{refreshButton,feedbackButton,infoButton};
		for (Button button : actionButtons) {
			button.setOnClickListener(actionOnClickListener);
			button.setText(button.getText().toString().toUpperCase());
		}
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
		if (itemPosition != currentNavPosition) {
			thingResultListFragment
					.resetFragmentWithNewThingCategoryUrl(thingsCategoryBaseUrls[itemPosition]);
		}
		currentNavPosition = itemPosition;
		return true;
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		super.onCreateOptionsMenu(menu);
		if(!ThingiBrowseApplication.isRunningOnGoogleTV()) {
			getSupportMenuInflater().inflate(R.menu.menu, menu);
		}
		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		switch (item.getItemId()) {
		case android.R.id.home:
			return true;
		case R.id.refresh:
			refresh();
			return true;
		case R.id.feedback:
			feedback();
			return true;
		case R.id.info:
			info();
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

	private void refresh() {
		thingResultListFragment.refresh();
	}

	private void feedback() {
		startActivity(ActionBarHelper.getInstance().getFeedbackIntent(
				getApplicationContext()));
	}

	private void info() {
		startActivity(ActionBarHelper.getInstance().getInfoIntent(
				getApplicationContext()));
	}

	private OnClickListener categoryOnClickListener = new OnClickListener() {
		
		@Override
		public void onClick(View v) {
			for (int x = 0; x < categoryButtons.length; x++) {
				if (categoryButtons[x].equals(v)) {
					onNavigationItemSelected(x, (long) x);
				}
			}
		}
	};
	
	private OnClickListener actionOnClickListener = new OnClickListener() {
		
		@Override
		public void onClick(View v) {
			if(v.equals(refreshButton)) {
				refresh();
			} else if(v.equals(feedbackButton)) {
				feedback();
			}else if(v.equals(infoButton)) {
				info();
			}
		}
	};
}