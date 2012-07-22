/*   Copyright 2012 Mario Böhmer
 *
 *   Licensed under Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported (CC BY-NC-SA 3.0) 
 *   you may not use this file except in compliance with the License.
 *   You may obtain a copy of the License at
 *
 *       http://creativecommons.org/licenses/by-nc-sa/3.0/
 */
package com.blogspot.marioboehmer.thingibrowse.fragments;

import java.io.IOException;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import android.app.Activity;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.text.Html;
import android.text.TextUtils;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.ScrollView;
import android.widget.TextView;

import com.actionbarsherlock.app.SherlockFragment;
import com.blogspot.marioboehmer.thingibrowse.R;
import com.blogspot.marioboehmer.thingibrowse.ThingGalleryActivity;
import com.blogspot.marioboehmer.thingibrowse.ThingiBrowseApplication;
import com.blogspot.marioboehmer.thingibrowse.domain.Thing;
import com.blogspot.marioboehmer.thingibrowse.network.OnNetworkErrorListener;
import com.blogspot.marioboehmer.thingibrowse.network.ThingException;
import com.blogspot.marioboehmer.thingibrowse.network.ThingRequester;
import com.novoda.imageloader.core.loader.Loader;
import com.novoda.imageloader.core.model.ImageTag;
import com.novoda.imageloader.core.model.ImageTagFactory;

/**
 * {@link Fragment} showing the details of a {@link Thing}.
 * 
 * @author Mario Böhmer
 */
public class ThingDetailsFragment extends SherlockFragment {

	private static final String TAG = ThingDetailsFragment.class.getSimpleName();
	private static final String IMAGE_DETAIL_PAGE_URLS = "imageDetailPageUrls";
	private static final String THING_URL = "thingUrl";
	private static final String THING = "thing";

	private Thing thing;
	private String thingUrl;

	private ProgressBar progressBar;
	private ScrollView content;
	private TextView thingTitle;
	private ImageButton thingImageButton;
	private TextView thingCreatedBy;
	private TextView thingCreationDate;
	private TextView thingDescriptionLabel;
	private TextView thingDescription;
	private TextView thingInstructionsLabel;
	private TextView thingInstructions;
	private LinearLayout thingFilesSectionContainer;
	private LinearLayout thingFilesContainer;

	private Loader imageLoader;
	private ImageTagFactory imageTagFactory;
	private LayoutInflater layoutInflater;
	private OnNetworkErrorListener onNetworkErrorListener;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		if (savedInstanceState == null) {
			savedInstanceState = getActivity().getIntent().getExtras();
		}
		if (savedInstanceState != null) {
			thing = (Thing) savedInstanceState.get(THING);
			thingUrl = savedInstanceState.getString(THING_URL);
		}
		imageTagFactory = new ImageTagFactory(getActivity(), R.drawable.image_loading);
		imageTagFactory.setErrorImageId(R.drawable.info);
		imageLoader = ThingiBrowseApplication.getImageManager().getLoader();
	}

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		this.layoutInflater = inflater;

		View view = inflater
				.inflate(R.layout.thing_details_fragment, container);
		progressBar = (ProgressBar) view.findViewById(android.R.id.empty);
		content = (ScrollView) view.findViewById(R.id.content);
		thingTitle = (TextView) view.findViewById(R.id.thing_title);
		thingCreatedBy = (TextView) view.findViewById(R.id.thing_createdby);
		thingCreationDate = (TextView) view
				.findViewById(R.id.thing_creation_date);
		thingDescriptionLabel = (TextView) view
				.findViewById(R.id.thing_description_label);
		thingDescription = (TextView) view.findViewById(R.id.thing_description);
		thingInstructionsLabel = (TextView) view
				.findViewById(R.id.thing_instructions_label);
		thingInstructions = (TextView) view
				.findViewById(R.id.thing_instructions);
		thingFilesSectionContainer = (LinearLayout) view
				.findViewById(R.id.thing_files_section_container);
		thingFilesContainer = (LinearLayout) view
				.findViewById(R.id.thing_files_container);
		thingImageButton = (ImageButton) view
				.findViewById(R.id.thing_image_button);
		thingImageButton.setOnClickListener(thingImageOnClickListener);

		return view;
	}

	@Override
	public void onAttach(Activity activity) {
		super.onAttach(activity);
		try {
			onNetworkErrorListener = (OnNetworkErrorListener) activity;
		} catch (ClassCastException e) {
			throw new ClassCastException(activity.toString()
					+ " must implement OnNetworkErrorListener");
		}
	}

	@Override
	public void onStart() {
		super.onStart();
		if (thing != null) {
			updateView(thing);
		} else if (thingUrl != null) {
			loadThing(thingUrl);
		}
	}

	@Override
	public void onSaveInstanceState(Bundle outState) {
		outState.putSerializable(THING, thing);
		outState.putString(THING_URL, thingUrl);
		super.onSaveInstanceState(outState);
	}

	public void loadThing(String thingUrl) {
		this.thingUrl = thingUrl;

		showProgressBar();

		new AsyncTask<String, Void, Thing>() {

			@Override
			protected Thing doInBackground(String... params) {
				Thing result = null;
				try {
					result = ThingRequester.getInstance(
							getActivity().getApplicationContext()).getThing(
							params[0]);
				} catch (IOException e) {
					onNetworkErrorListener.onNetworkError();
				} catch (ThingException e) {
					Log.w(TAG,
							"There was a problem while loading the current thing: "
									+ e);
				}
				return result;
			}

			@Override
			protected void onPostExecute(Thing result) {
				if (result != null) {
					thing = result;
					updateView(thing);
				}
			}
		}.execute(thingUrl);
	}

	private void updateView(Thing thing) {
		ImageTag tag = imageTagFactory.build(thing.getThingImageUrl());
		thingImageButton.setTag(tag);
	    imageLoader.load(thingImageButton);

		thingTitle.setText(Html.fromHtml(thing.getThingTitle()));
		thingCreatedBy.setText(thing.getThingCreatedBy());
		thingCreationDate.setText(thing.getThingDate());

		showDescription(thing.getThingDescription());
		showInstructions(thing.getThingInstructions());
		showFiles(thing.getThingFiles());

		hideProgressBar();
	}

	private void showDescription(String description) {
		if (TextUtils.isEmpty(description)) {
			thingDescription.setVisibility(View.GONE);
			thingDescriptionLabel.setVisibility(View.GONE);
		} else {
			thingDescription.setText(Html.fromHtml(description.replaceAll("\n",
					"<br />")));
			thingDescription.setVisibility(View.VISIBLE);
			thingDescriptionLabel.setVisibility(View.VISIBLE);
		}
	}

	private void showInstructions(String instructions) {
		if (TextUtils.isEmpty(instructions)) {
			thingInstructions.setVisibility(View.GONE);
			thingInstructionsLabel.setVisibility(View.GONE);
		} else {
			thingInstructions.setText(Html.fromHtml(instructions.replaceAll(
					"\n", "<br />")));
			thingInstructions.setVisibility(View.VISIBLE);
			thingInstructionsLabel.setVisibility(View.VISIBLE);
		}
	}

	private void showFiles(Map<String, String[]> files) {
		if (files.isEmpty()) {
			thingFilesSectionContainer.setVisibility(View.GONE);
		} else {
			thingFilesContainer.removeAllViews();
			Iterator<Entry<String, String[]>> filesIterator = files.entrySet()
					.iterator();
			while (filesIterator.hasNext()) {
				Entry<String, String[]> file = filesIterator.next();
				String[] fileDetails = file.getValue();

				String name = fileDetails[0];
				String size = fileDetails[1];
				String imageUrl = fileDetails[2];

				View fileView = layoutInflater.inflate(
						R.layout.thing_file_view, null);

				ImageView fileImage = (ImageView) fileView
						.findViewById(R.id.thing_file_image);
				ImageTag tag = imageTagFactory.build(imageUrl);
				fileImage.setTag(tag);
			    imageLoader.load(fileImage);

				TextView fileName = (TextView) fileView
						.findViewById(R.id.thing_file_name);
				fileName.setText(name);

				TextView fileSize = (TextView) fileView
						.findViewById(R.id.thing_file_size);
				fileSize.setText(size);

				thingFilesContainer.addView(fileView);
			}
			thingFilesSectionContainer.setVisibility(View.VISIBLE);
		}
	}

	private void showProgressBar() {
		content.setVisibility(View.GONE);
		progressBar.setVisibility(View.VISIBLE);
	}
	
	private void hideProgressBar() {
		progressBar.setVisibility(View.GONE);
		content.setVisibility(View.VISIBLE);
	}

	OnClickListener thingImageOnClickListener = new OnClickListener() {
		
		@Override
		public void onClick(View v) {
			Intent galleryIntent = new Intent(
					getActivity().getApplicationContext(),
					ThingGalleryActivity.class);
			List<String> allImageDetailPageUrls = thing
					.getThingAllImageDetailPageUrls();
			galleryIntent.putExtra(IMAGE_DETAIL_PAGE_URLS, allImageDetailPageUrls
					.toArray(new String[allImageDetailPageUrls.size()]));
			startActivity(galleryIntent);
		}
	};
}
