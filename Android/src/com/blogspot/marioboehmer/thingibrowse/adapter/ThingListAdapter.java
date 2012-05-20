/*   Copyright 2012 Mario Böhmer
 *
 *   Licensed under Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported (CC BY-NC-SA 3.0) 
 *   you may not use this file except in compliance with the License.
 *   You may obtain a copy of the License at
 *
 *       http://creativecommons.org/licenses/by-nc-sa/3.0/
 */
package com.blogspot.marioboehmer.thingibrowse.adapter;

import java.util.ArrayList;
import java.util.List;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Adapter;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;

import com.blogspot.marioboehmer.thingibrowse.R;
import com.blogspot.marioboehmer.thingibrowse.domain.ThingResultListItem;
import com.blogspot.marioboehmer.thingibrowse.fragments.ThingResultListFragment;
import com.example.android.imagedownloader.ImageDownloader;

/**
 * {@link Adapter} implementation for the {@link ThingResultListFragment}'s
 * {@link ListView} of {@link ThingResultListItem}s.
 * 
 * @author Mario Böhmer
 */
public class ThingListAdapter extends BaseAdapter {

	private List<ThingResultListItem> thingResultList = new ArrayList<ThingResultListItem>();
	private LayoutInflater layoutInflater;
	private ImageDownloader imageDownloader;

	public ThingListAdapter(Context context) {
		this.layoutInflater = (LayoutInflater) context
				.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
		imageDownloader = new ImageDownloader();
		imageDownloader.setMode(ImageDownloader.Mode.CORRECT);
	}

	public List<ThingResultListItem> getThingResultList() {
		return thingResultList;
	}

	@Override
	public int getCount() {
		return thingResultList.size();
	}

	@Override
	public Object getItem(int position) {
		return thingResultList.get(position);
	}

	@Override
	public long getItemId(int position) {
		return position;
	}

	@Override
	public View getView(int position, View convertView, ViewGroup parent) {
		if (convertView == null) {
			convertView = layoutInflater.inflate(
					R.layout.thing_result_list_item, null);
		}
		ThingResultListItem thing = (ThingResultListItem) getItem(position);

		ImageView imageView = (ImageView) convertView.findViewById(R.id.image);
		imageDownloader.download(thing.getThingImageUrl(), imageView);

		TextView title = (TextView) convertView.findViewById(R.id.title);
		title.setText(thing.getThingTitle());

		TextView creator = (TextView) convertView.findViewById(R.id.creator);
		creator.setText(thing.getThingCreatedBy());

		TextView date = (TextView) convertView.findViewById(R.id.date);
		date.setText(thing.getThingTime());

		return convertView;
	}
}
