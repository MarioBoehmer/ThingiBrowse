package com.blogspot.marioboehmer.thingibrowse;

import android.app.Application;

import com.novoda.imageloader.core.ImageManager;
import com.novoda.imageloader.core.LoaderSettings;
import com.novoda.imageloader.core.LoaderSettings.SettingsBuilder;
import com.novoda.imageloader.core.cache.LruBitmapCache;

/**
 * Application implementation for ThingiBrowse.
 * 
 * @author Mario Böhmer
 */
public class ThingiBrowseApplication extends Application{

	private static ImageManager imageManager;
	private static boolean isRunningOnGoogleTV;
	
	@Override
	public void onCreate() {
		super.onCreate();
		LoaderSettings settings = new SettingsBuilder()
	      .withDisconnectOnEveryCall(true)
	      .withCacheManager(new LruBitmapCache(this))
	      .withConnectionTimeout(20000)
	      .withReadTimeout(30000)
	      .build(this);
	    imageManager = new ImageManager(this, settings);
	    imageManager.getFileManager().clean(this);
	    if (getPackageManager().hasSystemFeature("com.google.android.tv")) {
	    	isRunningOnGoogleTV = true;
		}
	}

	public static final ImageManager getImageManager() {
	    return imageManager;
	}
	
	public static final boolean isRunningOnGoogleTV() {
	    return isRunningOnGoogleTV;
	}
}
