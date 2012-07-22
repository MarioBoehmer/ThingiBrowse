/*   Copyright 2012 Mario Böhmer
 *
 *   Licensed under Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported (CC BY-NC-SA 3.0) 
 *   you may not use this file except in compliance with the License.
 *   You may obtain a copy of the License at
 *
 *       http://creativecommons.org/licenses/by-nc-sa/3.0/
 */
package com.blogspot.marioboehmer.thingibrowse;

import android.content.res.Configuration;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.widget.Toast;

import com.actionbarsherlock.app.SherlockFragmentActivity;
import com.actionbarsherlock.view.Menu;
import com.actionbarsherlock.view.MenuItem;
import com.blogspot.marioboehmer.thingibrowse.network.OnNetworkErrorListener;

/**
 * Used only for devices with a small layout space like phones. Loads the
 * details fragment for displaying selected thing details.
 * 
 * @author Mario Böhmer
 */
public class ThingDetailsActivity extends SherlockFragmentActivity implements
		OnNetworkErrorListener {

	private boolean wasNetworkErrorShown = false;

	@Override
	protected void onCreate(Bundle bundle) {
		super.onCreate(bundle);
		getSupportActionBar().setDisplayShowTitleEnabled(false);
		getSupportActionBar().setDisplayHomeAsUpEnabled(true);
		
		setContentView(R.layout.thing_details_activity);
		Fragment detailsFragment = getSupportFragmentManager()
				.findFragmentById(R.id.thingDetailsFragment);
		if (detailsFragment != null
				&& !detailsFragment.isInLayout()
				&& getResources().getConfiguration().orientation == Configuration.ORIENTATION_LANDSCAPE) {
			finish();
			return;
		}
	}

	@Override
	protected void onResume() {
		super.onResume();
		wasNetworkErrorShown = false;
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

	@Override
	public void onNetworkError() {
		if (!wasNetworkErrorShown) {
			runOnUiThread(new Runnable() {

				@Override
				public void run() {
					Toast.makeText(ThingDetailsActivity.this,
							getString(R.string.network_error),
							Toast.LENGTH_LONG).show();
				}
			});
			wasNetworkErrorShown = true;
		}
	}
}
