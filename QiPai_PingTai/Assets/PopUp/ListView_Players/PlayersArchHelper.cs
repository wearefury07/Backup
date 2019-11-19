using System.Collections.Generic;

public class PlayersArchHelper
{

    // Use this for initialization
    public static List<Arch> Update(List<Arch> list)
    {
        var temp = new List<Arch>();
        for (int i = 0; i < list.Count; i++)
        {
            int level = 0;
            if (i == 0)
            {
                list[i].name = TypeToName(list[i].type);
                if (list[i].actual >= 10000)
                    level = 5;
                else if (list[i].actual >= 5000)
                    level = 4;
                else if (list[i].actual >= 500)
                    level = 3;
                else if (list[i].actual >= 50)
                    level = 2;
                else if (list[i].actual >= 5)
                    level = 1;
                else
                    level = 0;

                list[i].image = "avatar_achievement_" + i.ToString() + level.ToString();
                temp.Add(list[i]);
            }
            else if (i == 1)
            {
                list[i].name = TypeToName(list[i].type);
                if (list[i].actual >= 5000)
                    level = 5;
                else if (list[i].actual >= 2500)
                    level = 4;
                else if (list[i].actual >= 250)
                    level = 3;
                else if (list[i].actual >= 25)
                    level = 2;
                else if (list[i].actual >= 5)
                    level = 1;
                else
                    level = 0;

                list[i].image = "avatar_achievement_" + i.ToString() + level.ToString();
                temp.Add(list[i]);
            }
            else if (i == 2)
            {
                list[i].name = TypeToName(list[i].type);
                if (list[i].actual >= 500)
                    level = 5;
                else if (list[i].actual >= 100)
                    level = 4;
                else if (list[i].actual >= 10)
                    level = 3;
                else if (list[i].actual >= 5)
                    level = 2;
                else if (list[i].actual >= 1)
                    level = 1;
                else
                    level = 0;

                list[i].image = "avatar_achievement_" + i.ToString() + level.ToString();
                temp.Add(list[i]);
            }
        }
        return temp;
    }

    public static string TypeToName(int type)
    {
        return listAchievementName[type];
    }

    private static Dictionary<int, string> listAchievementName = new Dictionary<int, string>
    {
        {0,"Thắng trận"},
        {1,"Thua trận"},
        {2,"Cầu hòa"},
        {3,"Hiếu chiến"},
        {4,"Chặt 2"},
        {5,"Ăn gà"},
        {6,"Liêng"},
        {7,"Sập làng"},
        {8,"Ù K"},
        {9,"Báo sâm"},
        {10,"Cướp chương"},
        {11,"Tới trắng"},
        {12,"Đút 3 bích"},
        {13,"10 Át cụ"},
        {14,"Sáp"},
        {15,"Sám chi cuối"},
        {16,"Ù tròn"},
    };
}

