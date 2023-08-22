using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class UserClass
{
    public long userid { get; private set; }
    public string password { get; private set; }

    public string username; // { get; private set; }

    public string nickname;
    public string catchphrase;

    public Sprite profilePicture;
    public ThemeColor profilePictureColor;
    public Sprite profileBanner;
    public ThemeColor profileBannerColor;

    public int coins;
    public int xp;
    public int level;

    // TODO: Methods for userid, password & changing, username, etc

}
