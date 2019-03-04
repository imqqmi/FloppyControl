$(document).ready(function(){
  var entityMap = {
    "&": "&amp;",
    "<": "&lt;",
    ">": "&gt;",
    '"': '&quot;',
    "'": '&#39;',
    "/": '&#x2F;'
  };

  // Shamelessly taken from moustache
  function escapeHtml(string) {
    return String(string).replace(/[&<>"'\/]/g, function (s) {
      return entityMap[s];
    });
  }

  (function search_matching_results() {
    $.each($('div.code-result'), function(key, value) {
      var id = $(value).data('id');

      $.ajax('/api/related_results/' + id + '/')
       .done(function(data, textStatus, jqXHR) {
            var html = [];
            $.each(data, function(key2, value2) {
              html.push('<li>' +
                        '<a href="/codesearch/view/' + value2['id'] + '/">' +
                        value2['filename'] + ' in ' + value2['reponame'] +
                        '</a> ' + value2['location'] +
                        '</li>');
            });

            $(value).find('div.code-matches').html('<h5>Matching Results</h5><ul class="list-unstyled">'+html.join('')+'</ul>');
            if(data.length > 1) {
              var viewSimilar = $(value).find('a.view-similar-link');
              viewSimilar.html('Show ' + data.length + ' matches');
              viewSimilar.removeClass('hidden');

              viewSimilar.click(function(e) {
                e.preventDefault();
                $(this).hide();
                $(value).find('div.code-matches').removeClass('hidden');
              });
            }
      });
    });
  })();

  (function redirect_jsfiddle() {
    $('#redirect_jsfiddle').click(function(e) {
      e.preventDefault();
      $.ajax('/codesearch/raw/' + $('#similar-files').data('id') + '/')
       .done(function(data, textStatus, jqXHR) {
        var form = '<textarea class="hidden" name="js">' + data + '</textarea>';
        $('<form action="http://jsfiddle.net/api/post/library/pure/" method="POST">'+form+'</form>').appendTo('body').submit();
       }).fail(function(xhr, ajaxOptions, thrownError) { 
        alert('Sorry was unable to redirect to JSFiddle.');
      });
    });
  })();

  (function codeview_matching_results(){
    $('#view-similar-link').click(function(e) {
      $('#related-results').toggle();
    });

    var dataid = $('#similar-files').data('id');

    if(dataid === undefined) {
      return;
    }

    $.ajax('/api/related_results/' + dataid + '/')
     .done(function(data, textStatus, jqXHR) {
        var html = [];
        $.each(data, function(key, value) {
          html.push('<li>' +
                    '<a href="/codesearch/view/' + value['id'] + '/">' +
                    value['filename'] + ' in ' + value['reponame'] +
                    '</a> ' + value['location'] +
                    '</li>');
        });
        $('#related-results-list').html('<h5>Matching Results</h5><ul class="list-unstyled">'+html.join('')+'</ul>');
        if(data.length > 1) {
          $('#similar-files').html('Show ' + data.length + ' matches');
          $('#view-similar-link').removeClass('hidden');
        }
     });
  })();

  (function filetree_results(){
    var dataid = $('#file-tree-button').data('id');
    if(dataid === undefined) {
      return;
    }
    $('#file-tree-link').removeClass('hidden');
    var filetreedata = null;
    $('#file-tree-link').click(function(e) {
      $('#file-tree').toggle();
      if(filetreedata == null) {
        $('#file-tree-list').html('<center><img src="/static/bar-loading.gif" /></center>');
        $.ajax('/api/directory_tree/' + dataid + '/')
         .done(function(data, textStatus, jqXHR) {
            filetreedata = data;
            $('#file-tree-list').html(data['tree']);
         });
      }
    });
  })();

  (function search_sliders(){
    var lineRangeSlider = $("#line-range-slider");
    var lineRangeSliderValue = $('#line-range-slider-value');
    var ccrRangeSlider = $("#ccr-range-slider");
    var ccrRangeSliderValue = $('#ccr-range-slider-value');
    var locInput = $('#loc');
    var loc2Input = $('#loc2');
    var ccrInput = $('#ccr');
    var ccr2Input = $('#ccr2');

    if(lineRangeSlider.length != 0) {
      lineRangeSlider.noUiSlider({
        start: [lineRangeSlider.data('loc'), lineRangeSlider.data('loc2')],
        step: 100,
        range: {
          'min': 0,
          'max': 10000
        }
      });

      lineRangeSlider.on({
        slide: function(){
          var values = $(this).val();
          var val1 = Math.floor(values[0]);
          var val2 = Math.floor(values[1]);

          if(val2 == 10000) {
            val2 = '∞';
          }

          var display = val1 + ' to ' + val2 + ' lines';
          if(val1 == 0 && val2 == 10000) {
            display = 'Any number of lines';
          }

          lineRangeSliderValue.html(display);
          locInput.val(val1);
          loc2Input.val(val2);
        }
      });
    }

    if(ccrRangeSlider.length != 0) {
      ccrRangeSlider.noUiSlider({
        start: [ccrRangeSlider.data('ccr'), ccrRangeSlider.data('ccr2')],
        step: 5,
        range: {
          'min': 0,
          'max': 100
        }
      });

      ccrRangeSlider.on({
        slide: function(){
          var values = $(this).val();
          var val1 = Math.floor(values[0]);
          var val2 = Math.floor(values[1]);

          var display = val1 + '% to ' + val2 + '% comments';
          ccrRangeSliderValue.html(display);
          ccrInput.val(val1);
          ccr2Input.val(val2);
        }
      });
    }
  })();

  (function back_to_top(){
    var offset = 220;
    var duration = 500;
    $(window).scroll(function() {
        if (jQuery(this).scrollTop() > offset) {
            jQuery('.back-to-top').fadeIn(duration);
        } else {
            jQuery('.back-to-top').fadeOut(duration);
        }
    });
    
    $('.back-to-top').click(function(event) {
        event.preventDefault();
        jQuery('html, body').animate({scrollTop: 0}, duration);
        return false;
    })
  })();

  (function language_filter() {
    $('#filter-languages').keyup( function(event) {
      if (event.keyCode == 13) {
         event.preventDefault();
       }

       var filterval = $('#filter-languages').val().toLowerCase().split(' ');


       $('.lanfilter').each( function(key, value) {
          value = $(value);
          var textval = value.text().toLowerCase();
          var ismatch = true;
          for(var i=0;i<filterval.length;i++) {
            if (textval.indexOf(filterval[i]) === -1) {
              ismatch = false;
            }
          }

          if (ismatch) {
            value.show();
          } else {
            value.hide();
          }

       });

     });
  })();

  (function search_within(){
    var coderesults = new Bloodhound({
      datumTokenizer: function(query) {
        console.log(query);
      },
      queryTokenizer: Bloodhound.tokenizers.whitespace,
      limit: 10,
      remote: {
        url: '/api/codesearch_I/?q=%QUERY repo:' +  $('#within-input').data('username') + '/' + $('#within-input').data('name'),
        filter: function(list) { // convert to objects
          return $.map(list.results, function(coderesult) { return coderesult; });
        },
        ajax: {
          beforeSend: function() { 
            $('.tt-hint').css('background-image', 'url("/static/ball-loading.gif")');
            $('.tt-hint').css('background-repeat', 'no-repeat');
            $('.tt-hint').css('background-position', 'right 10px center');
          },
          complete: function() { 
            $('.tt-hint').css('background-image', '');
          }
        }
      }
    });
     
    coderesults.initialize();
     
    $('#prefetch .typeahead').typeahead(null, {
      name: 'searchresults',
      displayKey: 'filename',
      source: coderesults.ttAdapter(),
      templates: {
        empty: [
          '<div class="empty-message">',
          '<h5>No results found :(</h5>',
          '</div>'
        ].join('\n'),
        suggestion: function(result) { 
          var html = [
              '<a href="/codesearch/view/'+ result.id +'">',
              '<h5 class="within">' + result.location + '/' + result.filename,
              '</a>',
              ' <small>' + result.linescount + ' lines</small>',
              '<small> | ' + result.language + '</small>',
              '</h5>'
          ].join('\n');

          var lines = '<ol class="code-result">' + 
                        $.map(result.lines, function(line, lineno) { 
                          return ['<a href="/codesearch/view/' + result.id + '#l-' + lineno + '">',
                                 '<li value="' +  lineno + '">',
                                 '<pre>' + escapeHtml(line) + '</pre>',
                                 ' </a></li>'].join('\n'); 
                        }).join('') +
                      '</ol>';
          return html + lines;
        }
      }
    });
  })();
});