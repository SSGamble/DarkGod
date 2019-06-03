/****************************************************
	文件：EntityMonster.cs
	作者：CaptainYun
	日期：2019/06/03 10:14   	
	功能：怪物逻辑实体
*****************************************************/

public class EntityMonster : EntityBase {

    public MonsterData md;

    /// <summary>
    /// 等级影响
    /// </summary>
    /// <param name="props"></param>
    public override void SetBattleProps(BattleProps props) {
        int level = md.mLevel;
        BattleProps p = new BattleProps {
            hp = props.hp * level,
            ad = props.ad * level,
            ap = props.ap * level,
            addef = props.addef * level,
            apdef = props.apdef * level,
            dodge = props.dodge * level,
            pierce = props.pierce * level,
            critical = props.critical * level
        };
        Props = p;
        HP = p.hp;
    }
}