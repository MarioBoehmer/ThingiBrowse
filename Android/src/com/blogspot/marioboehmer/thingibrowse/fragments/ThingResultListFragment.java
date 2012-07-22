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
import java.util.ArrayList;

import android.app.Activity;
import android.os.AsyncTask;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AbsListView;
import android.widget.AbsListView.OnScrollListener;
import android.widget.ListView;

import com.actionbarsherlock.app.SherlockListFragment;
import com.blogspot.marioboehmer.thingibrowse.R;
import com.blogspot.marioboehmer.thingibrowse.ThingResultListActivity;
import com.blogspot.marioboehmer.thingibrowse.adapter.ThingListAdapter;
import com.blogspot.marioboehmer.thingibrowse.domain.ThingResultListItem;
import com.blogspot.marioboehmer.thingibrowse.network.OnNetworkErrorListener;
import com.blogspot.marioboehmer.thingibrowse.network.ThingException;
import com.blogspot.marioboehmer.thingibrowse.network.ThingRequester;

/**
 * {@link Fragment} showing a list of {@link ThingResultListItem}s.
 * 
 * @author Mario Böhmer
 */
public class ThingResultListFragment extends SherlockListFragment implements
		OnScrollListener {

	private static final String TAG = ThingResultListFragment.class
			.getSimpleName();
	private static final String SEARCH_TERM = "searchTerm";
	private static final String THING_RESULT_LIST = "thingResultList";
	private static final String IS_SEARCH = "isSearch";
	private static final String THING_CATEGORY_BASE_URL = "thingCategoryBaseUrl";
	private static final String CURRENT_PAGE = "currentPage";
	private static final String LAST_PAGE_INDEX = "lastPageIndex";

	private OnThingSelectedListener onThingSelectedListener;
	private OnNetworkErrorListener onNetworkErrorListener;
	private OnNoResultsListener onNoResultsListener;

	private ThingListAdapter thingListAdapter;
	private ArrayList<ThingResultListItem> thingResultList = new ArrayList<ThingResultListItem>();

	private String thingCategoryBaseUrl;
	private String searchTerm;
	private boolean isLoading = false;
	private boolean isSearchResultList = false;

	private int currentPage = 1;
	private int lastPageIndex = -1;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		thingListAdapter = new ThingListAdapter(getActivity());
		setListAdapter(thingListAdapter);
		if (savedInstanceState == null) {
			if (thingCategoryBaseUrl == null) {
				thingCategoryBaseUrl = getResources().getStringArray(
						R.array.things_category_base_urls)[0];
			}
			loadThingResults(currentPage);
		} else {
			currentPage = savedInstanceState.getInt(CURRENT_PAGE);
			lastPageIndex = savedInstanceState.getInt(LAST_PAGE_INDEX);
			thingCategoryBaseUrl = savedInstanceState
					.getString(THING_CATEGORY_BASE_URL);
			isSearchResultList = savedInstanceState.getBoolean(IS_SEARCH);
			searchTerm = savedInstanceState.getString(SEARCH_TERM);
			updateView((ArrayList<ThingResultListItem>) savedInstanceState.get(THING_RESULT_LIST), true);
		}
	}

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		return inflater.inflate(R.layout.thing_result_list_fragment, container);
	}

	@Override
	public void onActivityCreated(Bundle savedInstanceState) {
		super.onActivityCreated(savedInstanceState);
		getListView().setOnScrollListener(this);
	}

	@Override
	public void onAttach(Activity activity) {
		super.onAttach(activity);
		try {
			onThingSelectedListener = (OnThingSelectedListener) activity;
		} catch (ClassCastException e) {
			throw new ClassCastException(activity.toString()
					+ " must implement OnThingSelectedListener");
		}
		try {
			onNetworkErrorListener = (OnNetworkErrorListener) activity;
		} catch (ClassCastException e) {
			throw new ClassCastException(activity.toString()
					+ " must implement OnNetworkErrorListener");
		}
		try {
			onNoResultsListener = (OnNoResultsListener) activity;
		} catch (ClassCastException e) {
			throw new ClassCastException(activity.toString()
					+ " must implement OnNoResultsListener");
		}
	}

	@Override
	public void onSaveInstanceState(Bundle outState) {
		outState.putSerializable(THING_RESULT_LIST, thingResultList);
		outState.putInt(CURRENT_PAGE, currentPage);
		outState.putInt(LAST_PAGE_INDEX, lastPageIndex);
		outState.putString(THING_CATEGORY_BASE_URL, thingCategoryBaseUrl);
		outState.putBoolean(IS_SEARCH, isSearchResultList);
		outState.putString(SEARCH_TERM, searchTerm);
		super.onSaveInstanceState(outState);
	}

	@Override
	public void onListItemClick(ListView l, View v, int position, long id) {
		ThingResultListItem thingResultListItem = (ThingResultListItem) l.getAdapter().getItem(position);
		onThingSelectedListener.onThingSelected(thingResultListItem.getThingUrl());
	}

	public void refresh() {
		currentPage = 1;
		lastPageIndex = -1;
		thingListAdapter.getThingResultList().clear();
		thingListAdapter.notifyDataSetInvalidated();
		loadThingResults(currentPage);
	}

	private void loadThingResults(int pageNumber) {
		new AsyncTask<Integer, Void, ArrayList<ThingResultListItem>>() {

			@Override
			protected ArrayList<ThingResultListItem> doInBackground(
					Integer... params) {
				isLoading = true;
				ArrayList<ThingResultListItem> thingResultList = new ArrayList<ThingResultListItem>();
				try {
					String html = null;
					if (isSearchResultList) {
						html = ThingRequester.getInstance(
								getActivity().getApplicationContext())
								.getResponseHtmlForSearchThingResultList(
										params[0], searchTerm);
					} else {
						html = ThingRequester.getInstance(
								getActivity().getApplicationContext())
								.getResponseHtmlForThingResultList(
										thingCategoryBaseUrl, params[0]);
					}
					thingResultList.addAll(ThingRequester.getInstance(
							getActivity().getApplicationContext())
							.getThingResultList(html, isSearchResultList));
					if (currentPage == 1) {
						lastPageIndex = ThingRequester.getInstance(
								getActivity().getApplicationContext())
								.getThingResultListLastPageIndex(html,
										isSearchResultList);
					}
				} catch (IOException e) {
					onNetworkErrorListener.onNetworkError();
				} catch (ThingException e) {
					Log.w(TAG,
							"There was a problem while loading the ThingResultList: "
									+ e);
				} catch (Exception e) {
					Log.w(TAG, e);
				}
				return thingResultList;
			}

			@Override
			protected void onPostExecute(ArrayList<ThingResultListItem> result) {
				if (!result.isEmpty()) {
					isLoading = false;
					if (currentPage == 1) {
						updateView(result, true);
						if (((ThingResultListActivity) getActivity()).isDualFragmentsLayout) {
							onListItemClick(getListView(), null, 0, 0);
						}
					} else {
						updateView(result, false);
					}
				} else {
					showNoResults();
				}
			}
		}.execute(pageNumber);
	}

	private void updateView(
			ArrayList<ThingResultListItem> thingResultListItems,
			boolean clearList) {
		if (clearList) {
			thingListAdapter.getThingResultList().clear();
			thingListAdapter.notifyDataSetInvalidated();
		}
		thingListAdapter.getThingResultList().addAll(thingResultListItems);
		thingListAdapter.notifyDataSetChanged();
		thingResultList = (ArrayList<ThingResultListItem>) thingListAdapter
				.getThingResultList();
	}

	private void showNoResults() {
		onNoResultsListener.onNoResults();
	}

	@Override
	public void onScrollStateChanged(AbsListView view, int scrollState) {
		// not implemented
	}

	@Override
	public void onScroll(AbsListView view, int firstVisibleItem,
			int visibleItemCount, int totalItemCount) {
		boolean loadNextDataSet = firstVisibleItem + visibleItemCount >= totalItemCount;
		if (loadNextDataSet && totalItemCount > 0 && !isLoading) {
			currentPage++;
			if (currentPage <= lastPageIndex) {
				isLoading = true;
				loadThingResults(currentPage);
			}
		}
	}

	public void resetFragmentWithNewThingCategoryUrl(String newCategoryUrl) {
		this.thingCategoryBaseUrl = newCategoryUrl;
		isSearchResultList = false;
		refresh();
	}

	public void loadSearchResults(String searchTerm) {
		this.searchTerm = searchTerm;
		isSearchResultList = true;
		refresh();
	}

	public boolean isSearchResultList() {
		return isSearchResultList;
	}

	public interface OnThingSelectedListener {
		public void onThingSelected(String thingUrl);
	}

	public interface OnNoResultsListener {
		public void onNoResults();
	}
}
