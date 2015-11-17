
class /*CLASSNAME*/
{
public:
	/*PROPERTY*/

	/*CLASSNAME*/(void)
	{
		/*FIELDINIT*/
	}
	~/*CLASSNAME*/(void){};

	int getIntKey()
	{
		/*GETINTKEY*/
	}

	static /*CLASSNAME*/ create(std::map<string,string> /*ROW*/)
	{
		/*CREATEROWCODE*/

		return /*RESULT*/;
	}

	static std::vector</*CLASSNAME*/> create(std::vector<std::map<string,string>> table)
	{
		std::vector</*CLASSNAME*/> result;

		for(auto i=table.begin();i!=table.end();++i)
		{
			auto /*ROW*/=*i;

			/*CREATEROWCODE*/

			result.push_back(/*RESULT*/);
		}

		return result;
	}

	static std::vector</*CLASSNAME*/> where(std::string sCond)
	{
		std::string sSql="";
		sSql.append("select * from ");
		sSql.append(" /*CLASSNAME*/ ");
		sSql.append(" where ");
		sSql.append(sCond);

		return /*CLASSNAME*/::create(DB::sharedDB()->getDataSetFromTable(sSql));
	}
};

