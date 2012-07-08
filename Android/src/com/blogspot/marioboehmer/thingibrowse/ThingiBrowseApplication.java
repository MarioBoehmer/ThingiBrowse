package com.blogspot.marioboehmer.thingibrowse;

import android.app.Application;

import com.novoda.imageloader.core.ImageManager;
import com.novoda.imageloader.core.LoaderSettings;
import com.novoda.imageloader.core.LoaderSettings.SettingsBuilder;
import com.novoda.imageloader.core.cache.LruBitmapCache;

public class ThingiBrowseApplication extends Application{

	private static ImageManager imageManager;
	
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
	}

	public static final ImageManager getImageManager() {
	    return imageManager;
	}
}
