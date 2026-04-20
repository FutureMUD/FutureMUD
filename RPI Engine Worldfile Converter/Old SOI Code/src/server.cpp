#include "server.h"
#include <cstdlib>


rpie::server::server ()
{
  // Set Defaults
  // Override these in your config file.
  set_config ("mysql_host", "localhost");
  set_config ("mysql_user","kithrater");
  set_config ("mysql_passwd", "sevenas7");
  
  set_config ("engine_db", "firstage");
  set_config ("player_db", "firstage_player");
  set_config ("player_log_db", "firstage_world");
  set_config ("world_db", "firstage_world_log");
  set_config ("world_log_db", "firstage_auction");
  
  set_config ("server_mode", "test");
  set_config ("server_port", "4446");

  set_config ("server_path_play", "/home/kithrater/MUD/pp");
  set_config ("server_path_build", "/home/kithrater/MUD/bp");
  set_config ("server_path_test", "/home/kithrater/MUD/tp");
  return;
}

void
rpie::server::set_config (std::string var_name, std::string var_value = "true")
{
  if (var_value.size())
    {
      config_variables[var_name] = var_value;
      if (var_name == "server_mode")
	{
	  if (var_value == "play")
	    {
	      server_mode = mode_play;
	    }
	  else if (var_value == "build")
            {
              server_mode = mode_build;
            }
	  else if (var_value == "test")
            {
              server_mode = mode_test;
            }
          else
            {
              server_mode = mode_unknown;
            }
	}
      else if (var_name == "server_port")
	{
	  server_port = strtol (var_value.c_str (), 0, 10);
	}
    }
}

/// Load configuration files from the usual places
void
rpie::server::load_config_files ()
{
  std::vector<std::string> config_filenames;
  
  /// Push back filenames.
  config_filenames.push_back ("./.rpi_engine/config");
  config_filenames.push_back ("~/.rpi_engine/config");
  config_filenames.push_back ("/etc/rpi_engine/config");
  typedef std::vector<std::string>::const_iterator VI;
  for (VI i = config_filenames.begin ();
       i != config_filenames.end (); ++i)
    {
      std::ifstream config_file ((*i).c_str());
      if (config_file.is_open ())
	{
	  load_config_file (config_file);
	}
    }
}

/// Get the db connection info from the configuration file
void
rpie::server::load_config_file (std::ifstream &config_file)
{
  std::string line;
  while (std::getline (config_file,line))
    {
      // Remove comments
      std::string::size_type comment_start = line.find_first_of ('#');
      if (comment_start != std::string::npos)
	{
	  line.erase (comment_start, std::string::npos);
	}
      if (line.empty ())
	{
	  continue;
	}
      
      ///\TODO Make a more robust parser
      
      // we place delimiter '=' first so we can skip it easy
      const char * const delimiters = "= ;\t\r\n";
      std::string var_name;
      
      // skip whitespace to start of variable
      std::string::size_type var_start = 
	line.find_first_not_of (delimiters+1);
      if (var_start == std::string::npos)
	{
	  continue;
	}
      
      // find end of variable name
      std::string::size_type var_end =
	line.find_first_of (delimiters, var_start);
      if (var_end == std::string::npos) 
	{
	  var_name.assign (line.substr (var_start));
	  set_config (var_name);
	  continue;
	}
      else
	{
	  // store var_name for later
	  std::string::size_type len = var_end - var_start;
	  var_name.assign (line.substr (var_start, len));
	}

      // skip whitespace to start of '='
      std::string::size_type eq_start = 
	line.find_first_not_of (delimiters+1, var_end);
      if (eq_start == std::string::npos)
	{
	  set_config (var_name);
	  continue;
	}
      if (line.at(eq_start) != '=')
	{
	  std::cerr << "Warning, bad syntax on line: " 
		    << line << std::endl;
	  continue;
	}

      // skip whitespace to start of '='
      std::string::size_type val_start = 
	line.find_first_not_of (delimiters+1, eq_start+1);
      if (val_start == std::string::npos)
	{
	  std::cerr << "Warning, bad syntax on line: " 
		    << line << std::endl;
	  continue;
	}
      
      std::string val_string;
      char next_c = line.at(val_start);
      if (next_c == '"' || next_c == '\'')
	{
	  ++val_start;
	  std::string::size_type val_end = 
	    line.find_first_of (next_c, val_start);
	  
	  if (val_end == std::string::npos)
	    {
	      std::cerr << "Warning, bad syntax on line: " 
			<< line << std::endl;
	      continue;
	    }
		  
	  val_string = line.substr (val_start, val_end - val_start);
	  set_config (var_name, val_string);
	}
    }
}

std::string
rpie::server::get_config ()
{
  std::string output 
    ("\n#6Current Game Configuration#0\n"
     "#6--------------------------#0\n");

  typedef std::map<std::string,std::string>::iterator Iter;
  for (Iter i = config_variables.begin(); i != config_variables.end(); i++)
    {
      output += "#2";
      output += i->first;
      output += " = #0\"";
      if (i->first.find("passwd") != std::string::npos)
	{
	  output += "********";
	}
      else
	{
	  output += i->second;
	}
      output += "\";\n";
    }
  return output;
}

std::string
rpie::server::get_base_path (std::string req_mode)
{
  if (req_mode == "current")
    {
      req_mode = get_config ("server_mode");
    }
  return get_config ("server_path_" + req_mode);
}
