package com.blogspot.marioboehmer.thingibrowse.fragments;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnKeyListener;
import android.view.ViewGroup;
import android.view.Window;
import android.view.WindowManager.LayoutParams;
import android.view.inputmethod.EditorInfo;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.TextView.OnEditorActionListener;

import com.actionbarsherlock.app.SherlockDialogFragment;
import com.blogspot.marioboehmer.thingibrowse.R;

/**
 * {@link Fragment} for showing a search dialog.
 * 
 * @author Mario Böhmer
 */
public class SearchDialogFragment extends SherlockDialogFragment implements
		OnEditorActionListener, OnKeyListener {

	private EditText searchTermEditText;

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		View view = inflater.inflate(R.layout.thing_search_dialog_fragment,
				container);
		searchTermEditText = (EditText) view.findViewById(R.id.search_term);
		
		// title is set with custom TextView in layout, because Honeycomb 3.2
		// on GoogleTV has a bug displaying the normal title
		getDialog().requestWindowFeature(Window.FEATURE_NO_TITLE);

		// Show soft keyboard automatically
		searchTermEditText.requestFocus();
		getDialog().getWindow().setSoftInputMode(
				LayoutParams.SOFT_INPUT_STATE_VISIBLE);
		searchTermEditText.setOnEditorActionListener(this);
		searchTermEditText.setOnKeyListener(this);
		return view;
	}

	@Override
	public boolean onEditorAction(TextView v, int actionId, KeyEvent event) {
		if (EditorInfo.IME_ACTION_DONE == actionId) {
			return submitSearchTerm();
		}
		return false;
	}

	@Override
	public boolean onKey(View v, int keyCode, KeyEvent event) {
		if (event.getKeyCode() == KeyEvent.KEYCODE_ENTER
				|| event.getKeyCode() == KeyEvent.KEYCODE_DPAD_CENTER) {
			return submitSearchTerm();
		}
		return false;
	}

	private boolean submitSearchTerm() {
		// Return input text to activity
		SearchTermDialogListener activity = (SearchTermDialogListener) getActivity();
		activity.onFinishSearchDialog(searchTermEditText.getText().toString());
		this.dismiss();
		return true;
	}

	public interface SearchTermDialogListener {
		void onFinishSearchDialog(String inputText);
	}
}
